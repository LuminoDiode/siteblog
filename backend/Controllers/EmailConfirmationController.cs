using backend.Models.API.Email;
using backend.Models.Database;
using backend.Repository;
using backend.Services;
using backend.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Consumes("application/json")]
	[Produces("application/json")]
	public sealed class EmailConfirmationController : ControllerBase
	{
		// used for checking if user requesting confs too often
		private List<(DateTime dt, int userId)> LastSendedConfirmations = new();
		private const double newConfirmationTimeoutMinutesConst = 1;

		private UserService _userRepository;
		//private EmailConfirmationService _emailConfirmationService;
		private BlogContext _blogContext;
		public EmailConfirmationController(UserService userRepository, /*EmailConfirmationService emailConfirmationService,*/ BlogContext blogContext)
		{
			_userRepository = userRepository;
			//_emailConfirmationService = emailConfirmationService;
			_blogContext = blogContext;
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail([FromBody][Required] EmailConfirmationRequest request)
		{
			var confirmed = await _userRepository.SetEmailConfirmedIfJwtIsCorrect(request.EmailConfirmationJWT);
			if (confirmed) return Ok();
			else return Problem();
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> SendConfirmationEmail([FromBody][Required] int userId)
		{
			if (this.CanUserAccessAccount(userId))
			{
				if (LastSendedConfirmations.Last(x => x.userId.Equals(userId)).dt
					.AddMinutes(newConfirmationTimeoutMinutesConst) < DateTime.UtcNow)
				{
					LastSendedConfirmations.Add((DateTime.UtcNow, userId));
					var result = await _userRepository.SendConfirmationEmailAsync(userId);

					if (result)
					{
						return Ok();
					}
					else
					{
						return NotFound();
					}
				}
				else
				{
					return Problem($"You need to wait at least {newConfirmationTimeoutMinutesConst} minutes interval before the next request.", statusCode: 429);
				}
			}
			else
			{
				return Forbid();
			}
		}
	}
}
