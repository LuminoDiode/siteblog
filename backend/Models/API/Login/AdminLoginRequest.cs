using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Login
{
	public class AdminLoginRequest
	{
		[Required]
		public string KeyPhrase { get; set; } = null!;
	}
}
