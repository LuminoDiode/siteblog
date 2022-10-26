using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Login
{
    public class RegistrationRequest:LoginRequest
    {
		public string? Username { get; set; }
    }
}
