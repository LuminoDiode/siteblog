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
		protected virtual JwtServiceSettingsProvider _settings { get; set; }
		protected JwtSecurityTokenHandler _tokenHandler; 

		public JwtService(SettingsProviderService settings)
		{
			this._settings = settings.JwtServiceSettings;
			this._tokenHandler = new JwtSecurityTokenHandler();
		}

		public string GenerateJwtToken(User user)
			=> GenerateJwtToken(new Claim[] { 
				new Claim(nameof(user.Id), user.Id.ToString()), 
				new Claim(nameof( user.Email), user.Email), 
				new Claim(nameof( user.Name),user.Name ?? String.Empty),
				new Claim(nameof(user.UserRole),user.UserRole)
			});

		public string GenerateJwtToken(IList<Claim> claims, DateTime? expires = null)
		{
			return _tokenHandler.WriteToken(_tokenHandler.CreateToken(new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = expires ?? DateTime.UtcNow.AddDays(_settings.tokenLifespanDays),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.signingKey)), SecurityAlgorithms.HmacSha512Signature)
			}));
		}
		public async Task<IDictionary<string, object>?> ValidateJwtToken(string token)
		{
			var result = await _tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.signingKey))
#if DEBUG
				/* Данный твик устанавливает шаг проверки валидации времени смерти токена.
				 * https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/dev/src/Microsoft.IdentityModel.Tokens/TokenValidationParameters.cs#L345
				 * По умолчанию 5 минут, для тестов это слишком долго.
				 */
				,
				ClockSkew = TimeSpan.Zero
#endif
		});
			return result.IsValid ? result.Claims : null;
		}
	}
}
