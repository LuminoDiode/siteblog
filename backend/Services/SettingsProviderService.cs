using backend.Models.Runtime;

namespace backend.Services
{
	/* Класс служит для повышения уровня абстракции в других сервисах.
	 * Обеспечивает получение настроек из appsettings и прочих файлов
	 * с последующей их записью в соответствующие модели.
	 */
	public class SettingsProviderService
	{
		protected IConfiguration _configuration;

		public virtual JwtServiceSettingsProvider JwtServiceSettings { get; protected set; }
		public virtual FileUrnServiceSettingsProvider FileUrnServiceSettings { get; protected set; }
		public virtual EmailConfirmationServiceSettingsProvider EmailConfirmationServiceSettings { get; protected set; }
		public virtual SmtpClientsProviderServiceSettingsProvider SmtpClientsProviderServiceSettings { get; protected set; }
		public virtual PasswordsCryptographyServiceSettingsProvider PasswordsCryptographyServiceSettings { get; protected set; }
		public virtual ResetPasswordServiceSettingsProvider ResetPasswordServiceSettings { get; protected set; }
		public virtual DraftServiceSettingsProvider DraftServiceSettings { get; protected set; }

		public SettingsProviderService(IConfiguration configuration)
		{
			this._configuration = configuration;
			this.JwtServiceSettings = new(_configuration);
			this.FileUrnServiceSettings = new(_configuration);
			this.EmailConfirmationServiceSettings = new(_configuration);
			this.SmtpClientsProviderServiceSettings = new(_configuration);
			this.PasswordsCryptographyServiceSettings = new(_configuration);
			this.ResetPasswordServiceSettings = new(_configuration);
			this.DraftServiceSettings=new(_configuration);
		}
	}

	public class JwtServiceSettingsProvider
	{
		protected IConfiguration _configuration;
		public JwtServiceSettingsProvider(IConfiguration configuration)
		 => _configuration = configuration;


		public virtual int tokenLifespanDays
			=> _configuration.GetValue<int>(nameof(JwtServiceSettings) + ':' + nameof(JwtServiceSettings.tokenLifespanDays), 360);
		public virtual string signingKey
			=> _configuration.GetValue<string>(nameof(JwtServiceSettings) + ':' + nameof(JwtServiceSettings.signingKey), @"OVERRIDE_ME");
		public virtual string issuer
			=> _configuration.GetValue<string>(nameof(JwtServiceSettings) + ':' + nameof(JwtServiceSettings.issuer), @"OVERRIDE_ME");
	}

	public class FileUrnServiceSettingsProvider
	{
		protected IConfiguration _configuration;
		public FileUrnServiceSettingsProvider(IConfiguration configuration)
			=> _configuration = configuration;

		public virtual string imageFileExtension
			=> _configuration.GetValue<string>(nameof(StorageSettings) + ':' + nameof(StorageSettings.imageFileExtension), @"jpeg");
		public virtual string staticFilesUrnPath
			=> _configuration.GetValue<string>(nameof(StorageSettings) + ':' + nameof(StorageSettings.staticFilesUrnPath), @"staticfiles");
		public virtual string imagesDirectoryPath
			=> _configuration.GetValue<string>(nameof(StorageSettings) + ':' + nameof(StorageSettings.imagesDirectoryPath), @"user_added/images");
	}

	public class EmailConfirmationServiceSettingsProvider
	{
		protected IConfiguration _configuration;
		public EmailConfirmationServiceSettingsProvider(IConfiguration configuration)
			=> _configuration = configuration;

		public virtual string ownDomain
			=> _configuration.GetValue<string>(nameof(EmailConfirmationServiceSettings) + ':' + nameof(EmailConfirmationServiceSettings.ownDomain), @"bruhcontent.ru");
		public virtual double linkLifespanDays
			=> _configuration.GetValue<double>(nameof(EmailConfirmationServiceSettings) + ':' + nameof(EmailConfirmationServiceSettings.linkLifespanDays), 10000d);
		public virtual string urlPathBeforeToken
				=> _configuration.GetValue<string>(nameof(EmailConfirmationServiceSettings) + ':' + nameof(EmailConfirmationServiceSettings.urlPathBeforeToken), @"/api/emailConfirmation/");

	}
	public class SmtpClientsProviderServiceSettingsProvider
	{
		protected readonly IConfiguration _configuration;
		public SmtpClientsProviderServiceSettingsProvider(IConfiguration configuration)
			=> _configuration = configuration;

		public virtual SmtpServerInfo[] smtpServerInfos
			=> _configuration.GetValue<SmtpServerInfo[]>(nameof(SmtpClientsProviderServiceSettings) + ':' + nameof(SmtpClientsProviderServiceSettings.smtpServers), new SmtpServerInfo[] { });
		public virtual int clientsRenewIntervalMinutes
			=> _configuration.GetValue<int>(nameof(SmtpClientsProviderServiceSettings) + ':' + nameof(SmtpClientsProviderServiceSettings.clientsRenewIntervalMinutes), 1);
	}

	public class PasswordsCryptographyServiceSettingsProvider
	{
		protected readonly IConfiguration _configuration;
		public PasswordsCryptographyServiceSettingsProvider(IConfiguration configuration)
			=> _configuration = configuration;

		public virtual int saltSizeBytes
			=> _configuration.GetValue<int>(nameof(PasswordsCryptographyServiceSettings) + ':' + nameof(PasswordsCryptographyServiceSettings.saltSizeBytes), 1);
	}
	public class ResetPasswordServiceSettingsProvider
	{
		protected readonly IConfiguration _configuration;
		public ResetPasswordServiceSettingsProvider(IConfiguration configuration)
			=> _configuration = configuration;

		public virtual string ownDomain
			=> _configuration.GetValue<string>(nameof(ResetPasswordServiceSettings) + ':' + nameof(ResetPasswordServiceSettings.ownDomain), @"bruhcontent.ru");
		public virtual double linkLifespanDays
			=> _configuration.GetValue<double>(nameof(ResetPasswordServiceSettings) + ':' + nameof(ResetPasswordServiceSettings.linkLifespanDays), 1d);
		public virtual string urlPathBeforeToken
				=> _configuration.GetValue<string>(nameof(ResetPasswordServiceSettings) + ':' + nameof(ResetPasswordServiceSettings.urlPathBeforeToken), @"/resetPassword/");
	}
	public class DraftServiceSettingsProvider
	{
		protected readonly IConfiguration _configuration;
		public DraftServiceSettingsProvider(IConfiguration configuration)
			=> _configuration = configuration;


		public virtual int maxEntitiesStoredInRam
			=> _configuration.GetValue<int>(nameof(DraftServiceSettings) + ':' + nameof(DraftServiceSettings.maxEntitiesStoredInRam), 100);
		public virtual double maxTimeEntityStoredInRamMinutes
			=> _configuration.GetValue<double>(nameof(DraftServiceSettings) + ':' + nameof(DraftServiceSettings.maxTimeEntityStoredInRamMinutes), 10);
		public virtual int maxEntitiesStoredInDb
				=> _configuration.GetValue<int>(nameof(DraftServiceSettings) + ':' + nameof(DraftServiceSettings.maxEntitiesStoredInDb),1000);
		public virtual double maxTimeEntityStoredInDbMinutes
			=> _configuration.GetValue<double>(nameof(DraftServiceSettings) + ':' + nameof(DraftServiceSettings.maxTimeEntityStoredInDbMinutes), 43200);
		public virtual double updateStoredEntitiesIntervalMinutes
			=>  _configuration.GetValue<double>(nameof(DraftServiceSettings) + ':' + nameof(DraftServiceSettings.updateStoredEntitiesIntervalMinutes), 10);

	}
}
