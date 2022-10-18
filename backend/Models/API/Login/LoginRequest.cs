using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Login
{
	public class LoginRequest
	{
		[Required]
		public string Email { get; set; } = null!;
		[Required]
		public string Password { get; set; } = null!;
	}
}
