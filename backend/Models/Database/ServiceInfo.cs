using Npgsql.Internal.TypeHandlers.FullTextSearchHandlers;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Database
{
	public class Service
	{
		[Key]
		public int Id { get; set; }

		public string Name { get; set; }

		public string? Description { get; set; }

		public string Url { get; set; }

		[ForeignKey(nameof(this.TopImage))]
		public int? TopImageId { get; set; }
		public Image? TopImage { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public NpgsqlTsVector NameVector { get; set; } = null!;

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public NpgsqlTsVector DescriptionVector { get; set; } = null!;

		public Service(string Name, string Url)
		{
			this.Name = Name;
			this.Url = Url;
		}

	}
}
