using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Login
{
	public class LoginRequest
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		[PasswordPropertyText]
		public string Password { get; set; } = null!;
	}
}
