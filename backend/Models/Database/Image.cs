using backend.Models.API.Common;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Database
{
	public class Image: IObjectWithId
	{
		[Key]
		public int Id { get; set; }
		public string? Name { get; set; }
	}
}
