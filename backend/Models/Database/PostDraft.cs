using NpgsqlTypes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Database
{
	public class PostDraft:IObjectWithId
	{
		[Key]
		public int Id { get; set; }

		public DateTime UpdatedDate { get; set; }

		[ForeignKey(nameof(this.TopImage))]
		public int? TopImageId { get; set; }
		public Image? TopImage { get; set; }

		public string Title { get; set; }
		public string? Subtile { get; set; }
		public string TextHTML { get; set; }


		[ForeignKey(nameof(this.OwnerUser))]
		public int? OwnerUserId { get; set; }
		public User? OwnerUser { get; set; }

		public PostDraft(string Title, string TextHTML)
		{
			this.Title = Title;
			this.TextHTML = TextHTML;
			UpdatedDate = DateTime.UtcNow;
		}
	}
}
