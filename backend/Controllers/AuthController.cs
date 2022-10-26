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
using backend.Services;

namespace backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Consumes("application/json")]
	[Produces("application/json")]
	public class AuthController : ControllerBase
	{
		protected readonly IConfiguration _configuration;
		protected readonly ILogger _logger;
		protected readonly JwtService _jwtService;
		protected readonly PasswordsCryptographyService _passwordsCryptographyService;
		protected readonly BlogContext _blogContext;
		protected readonly UserService _userService;
		protected readonly UserService.PasswordConstraints _passwordConstraints;



		public AuthController(
			BlogContext blogContext, 
			UserService userService,
			JwtService jwtService, 
			PasswordsCryptographyService passwordsCryptographyService, 
			IConfiguration config, 
			ILogger<AuthController> logger)
		{
			_configuration = config;
			_logger = logger;
			_jwtService = jwtService;
			_passwordsCryptographyService = passwordsCryptographyService;
			_blogContext = blogContext;
			_userService = userService;

			_passwordConstraints = new UserService.PasswordConstraints();
		}


		[HttpGet]
		[AllowAnonymous]
		[Route("passwordConstraints")]
		public IActionResult GetPasswordConstraints() => Ok(this._passwordConstraints);

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

			if (!_userService.CheckPasswordAsync(usr, request.Password))
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

			if (!_userService.ValidateNewPassword(request.Password, this._passwordConstraints))
				return BadRequest(new HumanResponse("Wrong password."));

			await _userService.CreateNewUserAsync(request);

			return await Login(new LoginRequest { Email = request.Email, Password = request.Password });
		}

		[HttpDelete]
		[Authorize]
		public async Task<IActionResult> Delete(/*[FromBody][Required] RegistrationRequest request*/)
		{
			throw new NotImplementedException();
		}
	}
}
