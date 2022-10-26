using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Email
{
	public class EmailConfirmationRequest
	{
		[Required]
		public string EmailConfirmationJWT { get; set; } = null!;
	}
}
