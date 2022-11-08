using backend.Models.Database;
using backend.Models.Runtime;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
	public class JwtService
	{
		// making this field virtual prop doesnt give anything coz _settings still needs exactly this type
		protected readonly SettingsProviderService _settingsProvider;
		protected virtual JwtServiceSettings _settings => _settingsProvider.JwtServiceSettings;

		private readonly JwtSecurityTokenHandler _tokenHandler;

		public JwtService(SettingsProviderService settingsProvider)
		{
			this._settingsProvider = settingsProvider;
			this._tokenHandler = new JwtSecurityTokenHandler();
		}

		public string GenerateJwtToken(User user)
			=> GenerateJwtToken(new Claim[] {
				new Claim(nameof(User.Id), user.Id.ToString()),
				new Claim(nameof(User.EmailAddress), user.EmailAddress),
				new Claim(nameof(User.Name),user.Name ?? String.Empty),
				new Claim(nameof(User.UserRole),user.UserRole)
			});

		public string GenerateJwtToken(IList<Claim> claims, DateTime? expires = null)
		{
#if DEBUG
			var Subject = new ClaimsIdentity(claims);
			var Expires = expires ?? DateTime.UtcNow.AddDays(_settings.tokenLifespanDays);
			var Issuer = _settings.issuer;
			var SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.signingKey)),
				SecurityAlgorithms.HmacSha512Signature);
#endif

			return _tokenHandler.WriteToken(_tokenHandler.CreateToken(new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = expires ?? DateTime.UtcNow.AddDays(_settings.tokenLifespanDays),
				Issuer = _settings.issuer,
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.signingKey)),
					SecurityAlgorithms.HmacSha512Signature)
			}));
		}
		public ClaimsPrincipal ValidateJwtToken(string token)
		{
			var result = _tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = _settings.issuer,
				ValidateAudience = false,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.signingKey))
#if DEBUG
				/* Данный твик устанавливает шаг проверки валидации времени смерти токена.
				 * https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/dev/src/Microsoft.IdentityModel.Tokens/TokenValidationParameters.cs#L339
				 * По умолчанию 5 минут, для тестов это слишком долго.
				 */
				,
				ClockSkew = TimeSpan.Zero
#endif
			}, out _);
			return result;
		}
	}
}
