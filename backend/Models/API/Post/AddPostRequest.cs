using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Requests
{
	public class AddPostRequest
	{
		public int? topImageId { get; set; }

		[Required]
		public string Title { get; set; } = null!;
		public string? Subtitle { get; set; }
		[Required]
		public string HtmlText { get; set; } = null!;
	}
}
