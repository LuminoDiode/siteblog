using backend.Models.API.Common;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Login
{
	public class LoginResponse: HumanResponse
	{
		[Required]
		public string BearerToken { get; set; } = null!;
	}
}
