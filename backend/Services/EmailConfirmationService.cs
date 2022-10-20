using backend.Models.Runtime;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Flurl;
using System.Security.Cryptography;
using System.Security.Claims;

namespace backend.Services
{

	/*	Класс создает ссылки для подтверждения почты.
	 *	Ссылки несут в себе ID пользователя + случайный набор символов
	 *	с целью исключить возможно связи известных данных с 
	 *	секретным ключем подписи.
	 *  Сервис использует название домена из appsettings. 
	 */
	public class EmailConfirmationService
	{
		public struct EmailConfirmationInfo
		{
			public string Email;
			public DateTime ValidThrough;

			public EmailConfirmationInfo(string Email, DateTime? ValidThrough = null)
			{
				this.Email = Email;
				this.ValidThrough = ValidThrough ?? DateTime.MaxValue;
			}
		}

		private EmailConfirmationServiceSettingsProvider _settings { get; set; }
		private JwtService _jwtService { get; set; }

		private const string emailClaimName = "useremail";

		public EmailConfirmationService(SettingsProviderService settings, JwtService jwtService)
		{
			this._settings = settings.EmailConfirmationServiceSettings;
			this._jwtService = jwtService;
		}

		public string CreateLinkForEmail(string Email)
		{
			var generatedJwt = _jwtService.GenerateJwtToken(
				new Claim[] { new Claim(emailClaimName, Email) },
				DateTime.UtcNow.AddDays(_settings.linkLifespanDays)
			);

			return Url.Combine(
				$"http://{_settings.ownDomain}",
				_settings.urlPathBeforeToken,
				generatedJwt
			);
		}

		public async Task<string?> GetInfoFromLink(string Link)
		{
			var jwt = await _jwtService.ValidateJwtToken(Url.Parse(Link).PathSegments.Last());

			if (jwt is not null)
			{
				if (jwt.TryGetValue(emailClaimName, out var email))
				{
					return (string)email;
				}
			}
			return null;
		}
	}
}
