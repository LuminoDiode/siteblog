using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Common
{
	public interface IObjectWithIndex
	{
		[Key]
		public int Id { get; set; }
	}
}
