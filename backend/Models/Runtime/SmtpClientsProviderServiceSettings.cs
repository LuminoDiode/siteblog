namespace backend.Models.Runtime
{
	public abstract class SmtpClientsProviderServiceSettings
	{
		public virtual SmtpServerInfo[] smtpServers { get; set; } = null!;
		public virtual double clientsRenewIntervalMinutes { get; set; }
	}
}
