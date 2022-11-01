using backend.Models.API.Common;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Requests
{
	public class DeletePostRequest 
	{
		[Required]
		public int Id { get; set; }
	}
}
