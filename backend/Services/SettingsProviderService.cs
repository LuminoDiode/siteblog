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
		public SettingsProviderService(IConfiguration configuration)
		{
			_configuration = configuration;
			this.JwtServiceSettings = new(_configuration);
			this.FileUrnServiceSettings = new(_configuration);
			this.EmailConfirmationServiceSettings = new(_configuration);
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
}
