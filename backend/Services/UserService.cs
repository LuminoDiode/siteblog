using backend.Models.API.Login;
using backend.Models.Database;
using backend.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Org.BouncyCastle.Asn1.Ocsp;

namespace backend.Services
{
	public class UserService
	{
		protected readonly BlogContext _blogContext;
		protected readonly PasswordsCryptographyService _passwordsCryptographyService;
		protected readonly JwtService _jwtService;


		public UserService(BlogContext blogContext, PasswordsCryptographyService passwordsCryptographyService, JwtService jwtService)
		{
			this._blogContext = blogContext;
			_passwordsCryptographyService = passwordsCryptographyService;
			_jwtService = jwtService;
		}


		public async Task<User?>TryFindAsync(string emailAddress)
		{
			return await _blogContext.Users.SingleOrDefaultAsync(x => x.EmailAddress.Equals(emailAddress));
		}
		public async Task<User?> TryFindAsync(int Id)
		{
			return await _blogContext.Users.FindAsync(Id);
		}


		public async Task<bool> CheckPasswordAsync(string userEmail, string password)
		{
			return CheckPasswordAsync(await _blogContext.Users.SingleOrDefaultAsync(x => x.EmailAddress.Equals(userEmail)), password);
		}
		public async Task<bool> CheckPasswordAsync(int userId, string password)
		{
			return CheckPasswordAsync(await _blogContext.Users.FindAsync(userId), password);
		}
		public bool CheckPasswordAsync(User? usr, string password)
		{
			if (usr is null || password is null) return false;

			return _passwordsCryptographyService.ConfirmPassword(password, usr.PasswordHash, usr.PasswordSalt);
		}

		public struct PasswordConstraints
		{
			public int MinLen = 6;
			public int MaxLen = 256;
			public bool DigitRequired = true;
			public bool LetterRequired = true;

			public PasswordConstraints()
			{

			}
		}
		public bool ValidateNewPassword(string password, PasswordConstraints? passwordConstraints = null)
		{
			if(passwordConstraints is null) passwordConstraints= new PasswordConstraints();

			if (password.Length < passwordConstraints.Value.MinLen 
				|| password.Length > passwordConstraints.Value.MaxLen)
				return false;

			if (passwordConstraints.Value.DigitRequired)
				if (!password.Any(x => char.IsDigit(x)))
					return false;

			if (passwordConstraints.Value.LetterRequired)
				if (!password.Any(x => char.IsLetter(x)))
					return false;

			return true;
		}

		public async Task<EntityEntry<User>> CreateNewUserAsync(RegistrationRequest request)
		{
			var passHash = _passwordsCryptographyService.HashPassword(request.Password, out var generatedSalt);

			User usr = new User {
				CreatedDate = DateTime.UtcNow,
				EmailAddress = request.Email,
				PasswordHash = passHash,
				PasswordSalt = generatedSalt,
				Name = request.Username
			};

			return await _blogContext.Users.AddAsync(usr);
		}

		public LoginResponse CreateLoginResponse(User usr, params string[] humanNotifications)
		{
			return new LoginResponse
			{
				Id = usr.Id,
				Name = usr.Name,
				UserRole = usr.UserRole,
				HumanNotifications = humanNotifications,
				BearerToken = _jwtService.GenerateJwtToken(usr),
				EmailConfirmed = usr.EmailConfirmed
			};
		}
	}
}
