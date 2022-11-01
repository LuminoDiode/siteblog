namespace backend.Models.Runtime
{
	public class SmtpServerInfo
	{
		// there are actually no default values for those props
		public virtual string smtpServerUrl { get; set; }
		public virtual int smtpServerSslPort { get; set; }
		public virtual int smtpServerTlsPort { get; set; }
		public virtual string smtpServerUserName { get; set; }
		public virtual string smtpServerPassword { get; set; }

		public SmtpServerInfo(string smtpServerUrl, int smtpServerTlsPort, int smtpServerSslPort, string smtpServerUserName, string smtpServerPassword)
		{
			this.smtpServerUrl = smtpServerUrl;
			this.smtpServerSslPort = smtpServerSslPort;
			this.smtpServerTlsPort = smtpServerTlsPort;
			this.smtpServerUserName = smtpServerUserName;
			this.smtpServerPassword = smtpServerPassword;
		}
	}
}
