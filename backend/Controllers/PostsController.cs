using backend.Models.API.Requests;
using backend.Models.API.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using backend.Services;
using backend.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using backend.Models.Database;

namespace backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Consumes("application/json")]
	[Produces("application/json")]
	public class PostsController : ControllerBase
	{
		protected readonly PostService _postService;
		public PostsController(PostService postService)
		{
			_postService = postService;
		}

		#region Anonymous GET
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetPosts([FromForm][Required] PostsInfoRequest request)
		{
			if (request.MaxCount > byte.MaxValue) return Problem("Too many posts required.", statusCode: StatusCodes.Status403Forbidden);

			return Ok(await _postService.GetPosts().Skip(request.StartIndex).Take(request.MaxCount).ToListAsync());
		}
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetPostsByRoute([FromRoute] int? startIndex, [FromRoute] int? maxCount)
		{
			return await GetPosts(new PostsInfoRequest { StartIndex = startIndex ?? 0, MaxCount = maxCount ?? 30 });
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetPost([FromForm][Required] int Id)
		{
			var found = await _postService.TryFindAsync(Id);
			return found is not null ? Ok(found) : NotFound();
		}
		[HttpGet]
		[AllowAnonymous]
		[Route("{id:int}")]
		public async Task<IActionResult> GetPostByRoute([FromRoute][Required] int Id)
		{
			return await GetPost(Id);
		}
		#endregion


		#region Authorize PUT/PATCH/DELETE
		[HttpPut]
		[Authorize]
		public IActionResult PutPost([FromForm][Required] IFormFile topImage, [FromForm][Required] AddPostRequest newPost)
		{
			if(!_postService.ValidateAndProcessPost(newPost, out var processed))
				return Problem("Post sended to server does not satisfy the requirements.", statusCode: StatusCodes.Status403Forbidden);
			

		}

		[HttpPatch]
		[Authorize]
		public IActionResult PatchPost([FromForm][Required] IFormFile topImage, [FromForm][Required] EditPostRequest metadata)
		{
			throw new NotImplementedException();
		}

		[HttpDelete]
		[Authorize]
		public IActionResult DeletePost([FromBody][Required] int Id)
		{
			throw new NotImplementedException();
		}
		#endregion

	}
}
