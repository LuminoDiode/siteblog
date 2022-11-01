using System.ComponentModel.DataAnnotations;

namespace backend.Models.Database
{
	[Obsolete]
	public interface IObjectWithId
	{
		[Key]
		public int Id { get; set; }
	}
}
