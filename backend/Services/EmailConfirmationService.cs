using backend.Models.Runtime;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Flurl;
using System.Security.Cryptography;
using System.Security.Claims;
using MimeKit;
using MailKit.Net.Smtp;
using Duende.IdentityServer.Models;
using static Duende.IdentityServer.Models.IdentityResources;

namespace backend.Services
{

	/*	Класс создает ссылки для подтверждения почты.
	 *	Ссылки несут в себе ID пользователя + случайный набор символов
	 *	с целью исключить возможно связи известных данных с 
	 *	секретным ключем подписи.
	 *  Сервис использует название домена из appsettings. 
	 */
	public sealed class EmailConfirmationService { 
		private EmailConfirmationServiceSettingsProvider _settings { get; set; }
		private JwtService _jwtService;
		private SmtpClientsProviderService _smtpClientsService;

		private const string emailClaimName = "EmailAddress";
		private const string noReplyString = "no-reply";
		private const string emailConfirmationSubject = "email-confirmation";
		private const string emailConfirmationText = "Procceed the link to confirm your email: ";

		public EmailConfirmationService(SettingsProviderService settings, JwtService jwtService, SmtpClientsProviderService smtpClientsService)
		{
			this._settings = settings.EmailConfirmationServiceSettings;
			this._jwtService = jwtService;
			this._smtpClientsService = smtpClientsService;
		}

		public string CreateLinkForEmail(string Email)
		{
			var generatedJwt = _jwtService.GenerateJwtToken(
				new Claim[] { new Claim(emailClaimName, Email) },
				DateTime.UtcNow.AddDays(_settings.linkLifespanDays)
			);

			return Url.Combine(
				$"https://{_settings.ownDomain}",
				_settings.urlPathBeforeToken,
				generatedJwt
			);
		}

		public string? GetInfoFromLink(string Link)
		{
			return _jwtService.ValidateJwtToken(Link.Substring(Link.LastIndexOf('/') + 1)).FindFirstValue(emailClaimName);
		}

		public async Task SendConfirmationEmailAsync(string UserEmail)
		{
			var emailMessage = new MimeMessage();

			emailMessage.From.Add(new MailboxAddress(String.Empty, noReplyString + '@' + _settings.ownDomain));
			emailMessage.To.Add(new MailboxAddress(String.Empty, UserEmail));
			emailMessage.Subject = emailConfirmationSubject;
			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = emailConfirmationText + CreateLinkForEmail(UserEmail)
			};

			foreach (var client in _smtpClientsService.GetClients())
			{
				try
				{
					await client.SendAsync(emailMessage);
					continue;
				}
				catch
				{

				}
			}
		}
	}
}
