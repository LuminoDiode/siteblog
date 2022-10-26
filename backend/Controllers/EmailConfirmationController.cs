using backend.Models.API.Email;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Consumes("application/json")]
	[Produces("application/json")]
	public class EmailConfirmationController
	{
		public EmailConfirmationController()
		{

		}

		[HttpPost]
		public async Task<IActionResult> ConfirmEmail([FromBody][Required] EmailConfirmationRequest request)
		{
			throw new NotImplementedException();
		}
	}
}
