using backend.Models.API.Common;
using backend.Models.API.Login;
using backend.Repository;
using backend.Services;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EzPasswordValidator;
using backend.Models.Database;
using backend.Models.API.User;
using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using backend.Services.Repositories;

namespace backend.Controllers
{
	public static class ControllerExtensions
	{
		public static bool CanUserAccessAccount(this ControllerBase	 ctr, int? id) =>
				((ctr.User.FindFirst(nameof(backend.Models.Database.User.Id))?.Value
					.Equals(id.ToString()) ?? false)
				|| (ctr.User.FindFirst(nameof(backend.Models.Database.User.UserRole))?.Value
					.Equals(backend.Models.Database.User.UserRoleAdminConst) ?? false)
				);
		public static bool IsUserAdmin(this ControllerBase ctr, int? id) =>
				((ctr.User.FindFirst(nameof(backend.Models.Database.User.UserRole))?.Value
					.Equals(backend.Models.Database.User.UserRoleAdminConst) ?? false)
				);
	}

	[ApiController]
	[Route("api/[controller]")]
	[Consumes("application/json")]
	[Produces("application/json")]
	public class AccountController : ControllerBase
	{
		protected readonly IConfiguration _configuration;
		protected readonly ILogger _logger;
		protected readonly JwtService _jwtService;
		protected readonly PasswordsCryptographyService _passwordsCryptographyService;
		protected readonly UserService _userService;


		public AccountController(
			UserService userService,
			JwtService jwtService,
			PasswordsCryptographyService passwordsCryptographyService,
			IConfiguration config,
			ILogger<AccountController> logger)
		{
			_configuration = config;
			_logger = logger;
			_jwtService = jwtService;
			_passwordsCryptographyService = passwordsCryptographyService;
			_userService = userService;
		}


		[HttpGet]
		[AllowAnonymous]
		[Route("passwordConstraints")]
		public IActionResult GetPasswordConstraints() => Ok(this._userService.PasswordConstraints);

		[HttpGet]
		[AllowAnonymous]
		[Route("usernameConstraints")]
		public IActionResult GetUsernameConstraints() => Ok(this._userService.UsernameConstraints);

		[HttpGet]
		[Authorize]
		public IActionResult Validate() => Ok();


		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody][Required] LoginRequest request)
		{
			var usr = await _userService.TryFindAsync(request.Email);

			if (usr is null)
				return new NotFoundObjectResult(new HumanResponse("Such login was not found on the server."));

			if (!_userService.CheckPassword(usr, request.Password))
				return new UnauthorizedObjectResult(new HumanResponse("Wrong password."));

			return Ok(_userService.CreateLoginResponse(usr));
		}

		[HttpPut]
		[AllowAnonymous]
		public async Task<IActionResult> Register([FromBody][Required] RegistrationRequest request)
		{
			var usr = await _userService.TryFindAsync(request.Email);

			if (usr is not null)
				return new ConflictObjectResult(new HumanResponse("Such login was found on the server."));

			if (!_userService.ValidateNewPassword(request.Password))
				return BadRequest(new HumanResponse("Password does not match the requirements."));

			var created = await _userService.CreateNewUserAsync(request);
			await _userService.SendConfirmationEmailAsync(created.Entity.EmailAddress);

			return await Login(new LoginRequest { Email = request.Email, Password = request.Password });
		}

		[HttpPatch]
		[Authorize]
		public async Task<IActionResult> EditAccount([FromBody][Required] UserPatchRequest patchRequest)
		{
			// validating the request user id
			if (patchRequest.Id is null)
			{
				if (int.TryParse(this.User.FindFirstValue(nameof(backend.Models.Database.User.Id)),
					out var authorizedId))
				{
					return BadRequest(new HumanResponse("Cannot determinate what account to edit: " +
						"request does not contain editable user id and authorized user has no id."));
				}
				else
				{
					patchRequest.Id = authorizedId;
				}
			}


			// checking the access
			if (!this.CanUserAccessAccount(patchRequest.Id.Value))
				return Forbid();

			// checking if the entity exists
			var found = await _userService.TryFindAsync(patchRequest.Id.Value);
			if (found is null)
			{
				return NotFound();
			}

			if(string.IsNullOrEmpty(patchRequest.NewPassword) && string.IsNullOrEmpty(patchRequest.NewName))
			{
				return BadRequest("Patch request fields was empty.");
			}

			if(!string.IsNullOrEmpty(patchRequest.NewPassword))
			{
				if (!_userService.ValidateNewPassword(patchRequest.NewPassword))
				{
					return BadRequest(new HumanResponse("Password does not match the requirements."));
				}

				await _userService.SetNewPasswordAsync(patchRequest.Id.Value, patchRequest.NewPassword);
			}

			if(!string.IsNullOrEmpty(patchRequest.NewName))
			{
				if (!_userService.ValidateUsername(patchRequest.NewName))
				{
					return BadRequest(new HumanResponse("Username does not match the requirements."));
				}

				await _userService.SetNewUsernameForUser(patchRequest.Id.Value, patchRequest.NewName);
			}

			return Ok();
		}

		[HttpDelete]
		[Authorize]
		[Route("{id:int}")]
		public async Task<IActionResult> DeleteByRoute([FromRoute][Required] int id) => await this.Delete(id);

		[HttpDelete]
		[Authorize]
		public async Task<IActionResult> Delete([FromBody][Required] int id)
		{
			// checking the access
			if (!this.CanUserAccessAccount(id))
				return Forbid();


			// checking if the entity exists
			var found = await _userService.TryFindAsync(id);
			if (found is null)
			{
				return NotFound();
			}

			// deleting
			await _userService.DeleteUserAsync(found);

			return Ok();
		}
	}
}
