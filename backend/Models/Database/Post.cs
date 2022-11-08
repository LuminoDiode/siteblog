using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Database
{
	public class Post : PostDraft
	{
		//[Key]
		//public int Id { get; set; }

		public DateTime CreatedDate { get; set; }
		//public DateTime? UpdatedDate { get; set; }

		//[ForeignKey(nameof(this.TopImage))]
		//public int? TopImageId { get; set; }
		//public Image? TopImage { get; set; }

		//public string Title { get; set; }
		//public string? Subtile { get; set; }
		//public string TextHTML { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public NpgsqlTsVector TitleVector { get; set; } = null!;
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public NpgsqlTsVector TextVector { get; set; } = null!;

		//[ForeignKey(nameof(this.OwnerUser))]
		//public int? OwnerUserId { get; set; }
		//public User? OwnerUser { get; set; }

		public Post(string Title, string TextHTML)
			:base(Title, TextHTML)
		{
			CreatedDate = DateTime.UtcNow;
		}
	}
}
