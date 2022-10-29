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
	public class EmailConfirmationService
	{
		protected EmailConfirmationServiceSettingsProvider _settings { get; set; }
		protected JwtService _jwtService;
		protected SmtpClientsProviderService _smtpClientsService;
		protected ILogger _logger;

		protected const string emailClaimName = "EmailAddress";
		protected const string actionClaimName = "Action";
		protected const string emailConfirmationClaimName = "EmailConfirmation";
		protected const string noReplyString = "no-reply";
		protected const string emailConfirmationSubject = "email-confirmation";
		protected const string emailConfirmationText = "Procceed the link to confirm your email: ";

		public EmailConfirmationService(
			SettingsProviderService settings,
			JwtService jwtService,
			SmtpClientsProviderService smtpClientsService,
			ILogger<EmailConfirmationService> logger)
		{
			this._settings = settings.EmailConfirmationServiceSettings;
			this._jwtService = jwtService;
			this._smtpClientsService = smtpClientsService;
			this._logger = logger;

			_logger.LogInformation($"The instance of {nameof(EmailConfirmationService)} created.");
		}

		public string CreateLinkForEmail(string email)
		{
			var generatedJwt = _jwtService.GenerateJwtToken(
				new Claim[] {
					new Claim(emailClaimName, email),
					/* Не факт, что следующий клайм в принципе нужен.
					 * Он добавлен с целью возможности однозначной идентификации JWT,
					 * созданного для подтверждения почты, ибо подпись вычисляется
					 * как хеш клаймов с ключем (HMAC).
					 */
					new Claim(actionClaimName,emailConfirmationClaimName)
				},
				DateTime.UtcNow.AddDays(_settings.linkLifespanDays)
			);

			return Url.Combine(
				_settings.ownDomain,
				_settings.urlPathBeforeToken,
				generatedJwt
			);
		}

		public string? GetEmailFromLinkIfValid(string link)
		{
			var payload = _jwtService.ValidateJwtToken(link.Substring(link.LastIndexOf('/') + 1));
			if (payload.FindFirstValue(actionClaimName) == emailConfirmationClaimName)
			{
				return payload.FindFirstValue(emailClaimName);
			}
			else
			{
				return null;
			}
		}

		public async Task SendConfirmationEmailAsync(string userEmail)
		{
			var emailMessage = new MimeMessage();
			var createdLink = CreateLinkForEmail(userEmail);

			emailMessage.From.Add(new MailboxAddress(String.Empty, noReplyString + '@' + _settings.ownDomain));
			emailMessage.To.Add(new MailboxAddress(String.Empty, userEmail));
			emailMessage.Subject = emailConfirmationSubject;
			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = emailConfirmationText + createdLink
			};

			_logger.LogInformation(
				$"Telling {nameof(SmtpClientsProviderService)} " +
				$"to send email-confirmation message to \'{userEmail}\' " +
				$"with confirmation link \'{createdLink}\'.");

			await _smtpClientsService.TryAllClientsToSend(emailMessage);
		}
	}
}
