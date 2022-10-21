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
	public class EmailConfirmationService : BackgroundService
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
		private JwtService _jwtService;
		private List<(SmtpServerInfo server, SmtpClient client)> _smtpClients;

		private const string emailClaimName = "EmailAddress";
		private const string noReplyString = "no-reply";
		private const string emailConfirmationSubject = "email-confirmation";
		private const string emailConfirmationText = "Procceed the link to confirm your email: ";

		public EmailConfirmationService(SettingsProviderService settings, JwtService jwtService)
		{
			this._settings = settings.EmailConfirmationServiceSettings;
			this._jwtService = jwtService;

			_smtpClients = new();
		}

		public async Task TryCreateClientsAsync(IEnumerable<SmtpServerInfo>? smtpServerInfos = null)
		{
			foreach (var smtpServerInfo in smtpServerInfos ?? _settings.smtpServerInfos)
			{
				var newClient = new SmtpClient();

				try // TLS first
				{
					await newClient.ConnectAsync(
						smtpServerInfo.smtpServerUrl,
						smtpServerInfo.smtpServerTlsPort,
						options: MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
				}
				catch // SSL else
				{
					try
					{
						await newClient.ConnectAsync(
							smtpServerInfo.smtpServerUrl,
							smtpServerInfo.smtpServerSslPort,
							options: MailKit.Security.SecureSocketOptions.SslOnConnect);
					}
					catch
					{
						continue;
					}
				}

				try
				{
					await newClient.AuthenticateAsync(
						smtpServerInfo.smtpServerUserName,
						smtpServerInfo.smtpServerPassword
					);
				}
				catch
				{
					continue;
				}

				_smtpClients.Add((smtpServerInfo, newClient));
			}
		}
		public async Task RecreateDisconnectedClients()
		{
			List<SmtpServerInfo> serversToRecreate = new();

			foreach (var clientInfoPair in _smtpClients)
			{
				await clientInfoPair.client.NoOpAsync();
				if (clientInfoPair.client.IsConnected && clientInfoPair.client.IsAuthenticated)
				{
					continue;
				}
				else
				{
					serversToRecreate.Add(clientInfoPair.server);
					await clientInfoPair.client.DisconnectAsync(true);
					_smtpClients.Remove(clientInfoPair);
				}
			}

			await TryCreateClientsAsync(serversToRecreate);
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

			foreach (var clientServerPair in _smtpClients)
			{
				try
				{
					await clientServerPair.client.SendAsync(emailMessage);
					continue;
				}
				catch
				{

				}
			}
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await TryCreateClientsAsync();
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(_settings.clientsRenewIntervalMinutes * 60 * 100);
				await RecreateDisconnectedClients();
			}
		}
	}
}
