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
using System.Security.Claims;

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
		public async Task<IActionResult> PutPost([FromForm][Required] AddPostRequest newPost, [FromForm] int? topImageId)
		{
			if(!_postService.ValidateAndProcessPost(newPost, out var processed))
				return Problem("Post sended to server does not satisfy the requirements.", statusCode: StatusCodes.Status403Forbidden);

			if (processed is null) throw new AggregateException();
			processed.topImageId = topImageId;

			var added = await _postService.AddPostAsync(processed, int.Parse(this.User.FindFirstValue(nameof(backend.Models.Database.User.Id))));

			return Ok(added.Entity);
		}

		[HttpPatch]
		[Authorize]
		public async Task< IActionResult> PatchPost([FromBody][Required] EditPostRequest editedPost)
		{
			var found = await _postService.TryFindAsync(editedPost.Id);
			if(found is null)
			{
				return NotFound();
			}

			if (!this.CanUserAccessAccount(found.OwnerUserId))
			{
				return Problem("You have no access to this entity.", statusCode: StatusCodes.Status403Forbidden);
			}

			if(!_postService.ValidateAndProcessPost(editedPost, out var processed))
			{
				return Problem("Post sended to server does not satisfy the requirements.", statusCode: StatusCodes.Status403Forbidden);
			}

			if(processed is null) throw new AggregateException();
			editedPost.Title = processed.Title;
			editedPost.Subtitle = processed.Subtitle;
			editedPost.HtmlText = processed.HtmlText;

			var edited = await _postService.PatchPostIfExistsAsync(editedPost);
			if(edited is not null)
			{
				return Ok(edited.Entity);
			}
			else
			{
				return NotFound();
			}
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
