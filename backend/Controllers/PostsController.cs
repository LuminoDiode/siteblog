using backend.Models.API.Requests;
using backend.Models.API.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Consumes("application/json")]
	[Produces("application/json")]
	public class PostsController
	{
		[HttpGet]
		[AllowAnonymous]
		public IEnumerable<PostInfoResponse> PostsInfo()
		{
			throw new NotImplementedException();
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("{id:int}")]
		public PostInfoResponse PostsInfo([FromRoute][Required] int Id)
		{
			throw new NotImplementedException();
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("{id:int}")]
		public PostResponse Posts([FromRoute][Required] int Id)
		{
			throw new NotImplementedException();
		}

		[HttpPut]
		[Authorize(Roles ="admin")]
		public IActionResult Posts([FromForm][Required] IFormFile topImage, [FromForm][Required] AddPostRequest metadata)
		{
			throw new NotImplementedException();
		}

		[HttpPatch]
		[Authorize(Roles = "admin")]
		public IActionResult Posts([FromForm][Required] IFormFile topImage, [FromForm][Required] EditPostRequest metadata)
		{
			throw new NotImplementedException();
		}


	}
}
