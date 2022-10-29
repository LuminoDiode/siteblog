using backend.Models.API.Login;
using backend.Models.Database;
using backend.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Ocsp;

namespace backend.Services
{
	// scoped service
	public class UserService
	{
		protected readonly PasswordsCryptographyService _passwordsCryptographyService;
		protected readonly JwtService _jwtService;
		protected readonly EmailConfirmationService _emailConfirmationService;
		protected readonly BlogContext _blogContext;

		public UserService( 
			BlogContext blogContext,
			PasswordsCryptographyService passwordsCryptographyService, 
			JwtService jwtService,
			EmailConfirmationService emailService)
		{
			_passwordsCryptographyService = passwordsCryptographyService;
			_jwtService = jwtService;
			_emailConfirmationService = emailService;
			_blogContext = blogContext;
		}

		public async Task<User?> TryFindAsync(string emailAddress)
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

		public struct UsernameConstraints
		{
			public int MinLen = 3;
			public int MaxLen = 32;
			public bool WhitespaceAllowed = false;
			public bool NonLetterOrDigitAllowed = false;

			public UsernameConstraints()
			{

			}
		}
		public bool ValidateUsername(string username, UsernameConstraints? usernameConstraints = null)
		{
			if (string.IsNullOrEmpty(username)) return false;

			if (usernameConstraints is null) usernameConstraints = new UsernameConstraints();

			if (username.Length < usernameConstraints.Value.MinLen
				|| username.Length > usernameConstraints.Value.MaxLen)
				return false;

			if (!usernameConstraints.Value.NonLetterOrDigitAllowed)
				if (username.Any(x => !(char.IsLetter(x) || char.IsDigit(x))))
					return false;

			if (!usernameConstraints.Value.WhitespaceAllowed)
				if (username.Any(x => char.IsWhiteSpace(x)))
					return false;

			return true;
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
			if (string.IsNullOrEmpty(password)) return false;

			if (passwordConstraints is null) passwordConstraints = new PasswordConstraints();

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

			User usr = new User
			{
				CreatedDate = DateTime.UtcNow,
				EmailAddress = request.Email,
				PasswordHash = passHash,
				PasswordSalt = generatedSalt,
				Name = request.Username
			};

			var added = await _blogContext.Users.AddAsync(usr);
			await _blogContext.SaveChangesAsync();
			return added;
		}

		public async Task<bool> SendConfirmationEmailAsync(int id)
		{
			var found = await _blogContext.Users.FindAsync(id);
			if (found is null) return false;
			await _emailConfirmationService.SendConfirmationEmailAsync(found.EmailAddress);
			return true;
		}
		public async Task SendConfirmationEmailAsync(string email)
		{
			await _emailConfirmationService.SendConfirmationEmailAsync(email);
		}

		/// <summary>
		/// Finds if the user exists, and if it is - deleting.
		/// </summary>
		/// <param name="Id">The deleted user Id.</param>
		/// <returns>true if user with such Id was marked for deletion, false otherwise.</returns>
		public async Task<bool> TryDeleteUserAsync(int Id)
		{
			var found = await _blogContext.Users.FindAsync(Id);
			if (found is null) return false;
			_blogContext.Users.Remove(found);
			await _blogContext.SaveChangesAsync();

			return true;
		}

		/// <summary>
		/// Finds if the user exists, and if it is - deleting.
		/// </summary>
		/// <param name="Id">The deleted user Id.</param>
		/// <returns>true if user with such Id was marked for deletion, false otherwise.</returns>
		public async Task<bool> TryDeleteUserAsync(string email)
		{
			var found = await TryFindAsync(email);
			if (found is null) return false;
			_blogContext.Users.Remove(found);
			return true;
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

		public async Task SetNewPasswordAsync(int userId, string newPassword)
		{
			await SetNewPasswordAsync(await TryFindAsync(userId), newPassword);
		}
		public async Task SetNewPasswordAsync(string userEmail, string newPassword)
		{
			await SetNewPasswordAsync(await TryFindAsync(userEmail), newPassword);
		}
		public async Task SetNewPasswordAsync(User? user, string newPassword)
		{
			if (user is null) throw new ArgumentException("User was null. Check if such user exists in the DB.");

			var hash = _passwordsCryptographyService.HashPassword(newPassword, out var salt);

			user.PasswordHash = hash;
			user.PasswordSalt = salt;

			await _blogContext.SaveChangesAsync();
		}


		public async Task SetNewUsernameForUser(int userId, string newName)
		{
			await SetNewUsernameForUser(await TryFindAsync(userId), newName);
		}
		public async Task SetNewUsernameForUser(string userEmail, string newName)
		{
			await SetNewUsernameForUser(await TryFindAsync(userEmail), newName);
		}
		public async Task SetNewUsernameForUser(User? user, string newUsername)
		{
			if (user is null) throw new ArgumentNullException("User was null. Check if such user exists in the DB.");

			user.Name = newUsername;
			await _blogContext.SaveChangesAsync();
		}

	}
}
