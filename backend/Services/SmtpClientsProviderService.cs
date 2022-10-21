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

namespace backend.Services
{
	public class SmtpClientsProviderService : BackgroundService
	{

		private SmtpClientsProviderServiceSettingsProvider _settings { get; set; }
		private List<(SmtpServerInfo server, SmtpClient client)> _smtpClients;

		public SmtpClientsProviderService(SettingsProviderService settings)
		{
			this._settings = settings.SmtpClientsProviderServiceSettings;

			_smtpClients = new();
		}

		public IEnumerable<SmtpClient> GetClients()
		{
			return _smtpClients.Select(x => x.client);
		}

		private async Task TryCreateClientsAsync(IEnumerable<SmtpServerInfo>? smtpServerInfos = null)
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
		private async Task RecreateDisconnectedClients()
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
