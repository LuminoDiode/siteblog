using backend.Models.API.Common;
using backend.Models.API.User;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Login
{
	public class LoginResponse: HumanResponse 
	{
		public string BearerToken { get; set; } = null!;
		public int Id { get; set; }
		public string? Name { get; set; }
		public bool EmailConfirmed { get; set; }
		public string UserRole { get; set; } = "user"; // user,admin,moderator
	}
}
