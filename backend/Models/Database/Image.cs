using System.ComponentModel.DataAnnotations;

namespace backend.Models.Database
{
	public class Image
	{
		[Key]
		public int Id { get; set; }
		public string? Name { get; set; }
	}
}
