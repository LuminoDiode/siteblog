namespace backend.Models.Runtime
{
	public class SmtpClientsProviderServiceSettings
	{
		public virtual SmtpServerInfo[] smtpServers { get; set; } = Array.Empty<SmtpServerInfo>();
		public virtual double clientsRenewIntervalMinutes { get; set; } = 10;
	}
}
