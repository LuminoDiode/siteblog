using backend.Models.API.User;
using backend.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Consumes("application/json")]
	[Produces("application/json")]
	public class UserController
	{
		[HttpGet]
		[AllowAnonymous]
		[Route("{Id:int}")]
		public UserInfoResponse GetUser([FromRoute][Required] int Id)
		{
			throw new NotImplementedException();
		}

		[HttpPatch]
		[Authorize]
		public StatusCodeResult EditUser([FromBody][Required] UserPatchRequest NewData)
		{
			throw new NotImplementedException();
		}

		[NonAction]
		public StatusCodeResult PutUser(User NewUser)
		{
			throw new NotImplementedException();
		}
	}
}
