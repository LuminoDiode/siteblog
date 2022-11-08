using backend.Models.Runtime;
using Flurl;
using MimeKit;
using System.Security.Claims;
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
using System.Threading;

namespace backend.Services
{
	public class SmtpClientsProviderService : BackgroundService
	{
		protected virtual SettingsProviderService _settingsProvider { get; init; }
		protected SmtpClientsProviderServiceSettings _settings => _settingsProvider.SmtpClientsProviderServiceSettings;
		protected List<(SmtpServerInfo server, SmtpClient client)> _smtpClients;
		protected ILogger _logger;

		public SmtpClientsProviderService(SettingsProviderService settingsProvider, ILogger<SmtpClientsProviderService>logger)
		{
			this._settingsProvider = settingsProvider;
			this._logger = logger;
			_smtpClients = new();

			_logger.LogInformation($"The instance of {nameof(SmtpClientsProviderService)} created.");
		}

		public IEnumerable<SmtpClient> GetClients()
		{
			return _smtpClients.Select(x => x.client);
		}

		protected async Task TryCreateClientsAsync(IEnumerable<SmtpServerInfo>? smtpServerInfos = null)
		{
			foreach (var smtpServerInfo in smtpServerInfos ?? _settings.smtpServers)
			{
				string protocol;
				var newClient = new SmtpClient();

				_logger.LogInformation(
						$"Trying to create new smtp client connected to \'{smtpServerInfo.smtpServerUrl}\' " +
						$"with username \'{smtpServerInfo.smtpServerUserName}\'.");

				try // TLS first
				{
					await newClient.ConnectAsync(
						smtpServerInfo.smtpServerUrl,
						smtpServerInfo.smtpServerTlsPort,
						options: MailKit.Security.SecureSocketOptions.StartTls);

					protocol = "TLS";
				}
				catch(Exception TlsEx) // SSL else
				{
					_logger.LogWarning(
						$"Failed connecting to smtp server at \'{smtpServerInfo.smtpServerUrl}\' " +
						$"with username \'{smtpServerInfo.smtpServerUserName}\' using TLS: {TlsEx.Message}. Now trying SSL...");

					try
					{
						await newClient.ConnectAsync(
							smtpServerInfo.smtpServerUrl,
							smtpServerInfo.smtpServerSslPort,
							options: MailKit.Security.SecureSocketOptions.SslOnConnect);

						protocol = "SSL";
					}
					catch(Exception SslEx)
					{
						_logger.LogError(
						$"Failed connecting to smtp server at \'{smtpServerInfo.smtpServerUrl}\' " +
						$"with username \'{smtpServerInfo.smtpServerUserName}\' using SSL: {SslEx.Message}.");

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
				catch(Exception AuthEx)
				{
					_logger.LogError(
						$"Failed to authenticate at \'{smtpServerInfo.smtpServerUrl}\' " +
						$"with username \'{smtpServerInfo.smtpServerUserName}\': {AuthEx.Message}.");

					continue;
				}


				_logger.LogInformation(
						$"Successfully connected and authenticated at \'{smtpServerInfo.smtpServerUrl}\' " +
						$"with username \'{smtpServerInfo.smtpServerUserName}\' using {protocol}.");

				_smtpClients.Add((smtpServerInfo, newClient));
			}
		}
		protected async Task RecreateDisconnectedClients()
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
					_logger.LogWarning(
						$"The smtp client connected to \'{clientInfoPair.server.smtpServerUrl}\' " +
						$"with username \'{clientInfoPair.server.smtpServerUserName}\' is marked " +
						$"for reconnection.");

					serversToRecreate.Add(clientInfoPair.server);
					await clientInfoPair.client.DisconnectAsync(true);
					_smtpClients.Remove(clientInfoPair);
				}
			}

			await TryCreateClientsAsync(serversToRecreate);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation($"Starting {nameof(SmtpClientsProviderService)} in the background.");

			await TryCreateClientsAsync();

			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay((int)(_settings.clientsRenewIntervalMinutes * 60 * 1000));
				await RecreateDisconnectedClients();
			}
		}

		public async Task TryAllClientsToSend(MimeMessage message)
		{
			foreach (var clientInfoPair in this._smtpClients)
			{
				try
				{
					await clientInfoPair.client.SendAsync(message);

					_logger.LogInformation(
						$"Successfully sended message to {message.To[0]} using smtp server at " +
						$"\'{clientInfoPair.server.smtpServerUrl}\' with username {clientInfoPair.server.smtpServerUserName}");

					break;
				}
				catch(Exception ex)
				{
					_logger.LogWarning(
						$"Failed to send message to {message.To[0]} using smtp server at " +
						$"\'{clientInfoPair.server.smtpServerUrl}\' with username {clientInfoPair.server.smtpServerUserName}: " +
						$"{ex.Message}");
				}

				_logger.LogError(
						$"Neither of {this._smtpClients.Count} smtp clients " +
						$"was able to send message to {message.To[0]}.");
			}
		}
	}
}
