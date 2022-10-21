namespace backend.Models.Runtime
{
	public abstract class SmtpServerInfo
	{
		public virtual string smtpServerUrl { get; set; } = null!;
		public virtual int smtpServerSslPort { get; set; }
		public virtual int smtpServerTlsPort { get; set; }
		public virtual string smtpServerUserName { get; set; } = null!;
		public virtual string smtpServerPassword { get; set; } = null!;
	}
}
