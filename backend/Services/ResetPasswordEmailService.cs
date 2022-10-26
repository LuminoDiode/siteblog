﻿using Flurl;
using MimeKit;
using System.Security.Claims;

namespace backend.Services
{
	public class ResetPasswordEmailService
	{
		protected ResetPasswordServiceSettingsProvider _settings;
		protected JwtService _jwtService;
		protected SmtpClientsProviderService _smtpClientsService;
		protected ILogger _logger;

		protected const string emailClaimName = "EmailAddress";
		protected const string actionClaimName = "Action";
		protected const string passwordResetClaimName = "PasswordReset";
		protected const string noReplyString = "no-reply";
		protected const string emailConfirmationSubject = "email-confirmation";
		protected const string emailConfirmationText = "Procceed the link to confirm your email: ";

		public ResetPasswordEmailService(
			SettingsProviderService settings,
			JwtService jwtService,
			SmtpClientsProviderService smtpClientsService,
			ILogger<ResetPasswordEmailService> logger)
		{
			this._settings = settings.ResetPasswordServiceSettings;
			this._jwtService = jwtService;
			this._smtpClientsService = smtpClientsService;
			this._logger = logger;

			_logger.LogInformation($"The instance of {nameof(ResetPasswordEmailService)} created.");
		}

		public string CreateLinkForReset(string email)
		{
			var generatedJwt = _jwtService.GenerateJwtToken(
				new Claim[] {
					new Claim(emailClaimName, email),
					/* Не факт, что следующий клайм в принципе нужен.
					 * Он добавлен с целью возможности однозначной идентификации JWT,
					 * созданного для подтверждения почты, ибо подпись вычисляется
					 * как хеш клаймов с ключем (HMAC).
					 */
					new Claim(actionClaimName,passwordResetClaimName)
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
			if (payload.FindFirstValue(actionClaimName) == passwordResetClaimName)
			{
				return payload.FindFirstValue(emailClaimName);
			}
			else
			{
				return null;
			}
		}

		public async Task SendResetPasswordEmailAsync(string userEmail)
		{
			var emailMessage = new MimeMessage();
			var createdLink = CreateLinkForReset(userEmail);

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

