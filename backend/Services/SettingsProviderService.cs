using backend.Models.Runtime;
using Microsoft.Extensions.Configuration;

namespace backend.Services
{
	/* Класс служит для повышения уровня абстракции в других сервисах.
	 * Обеспечивает получение настроек из appsettings и прочих файлов
	 * с последующей их записью в соответствующие модели.
	 */
	public class SettingsProviderService
	{
		protected IConfiguration _configuration;

		public virtual JwtServiceSettings JwtServiceSettings 
			=> _configuration.GetSection(nameof(JwtServiceSettings)).Get<JwtServiceSettings>();
		public virtual FileUrnServiceSettings FileUrnServiceSettings
			=> _configuration.GetSection(nameof(FileUrnServiceSettings)).Get<FileUrnServiceSettings>();
		public virtual EmailConfirmationServiceSettings EmailConfirmationServiceSettings
			=> _configuration.GetSection(nameof(EmailConfirmationServiceSettings)).Get<EmailConfirmationServiceSettings>();
		public virtual SmtpClientsProviderServiceSettings SmtpClientsProviderServiceSettings
			=> _configuration.GetSection(nameof(SmtpClientsProviderServiceSettings)).Get<SmtpClientsProviderServiceSettings>();
		public virtual PasswordsCryptographyServiceSettings PasswordsCryptographyServiceSettings
			=> _configuration.GetSection(nameof(PasswordsCryptographyServiceSettings)).Get<PasswordsCryptographyServiceSettings>();
		public virtual ResetPasswordServiceSettings ResetPasswordServiceSettings
			=> _configuration.GetSection(nameof(ResetPasswordServiceSettings)).Get<ResetPasswordServiceSettings>();
		public virtual DraftServiceSettings DraftServiceSettings
			=> _configuration.GetSection(nameof(DraftServiceSettings)).Get<DraftServiceSettings>();
		public virtual PostDraftServiceSettings PostDraftServiceSettings
			=> _configuration.GetSection(nameof(PostDraftServiceSettings)).Get<PostDraftServiceSettings>();
		public virtual UserServiceSettings UserServiceSettings
			=> _configuration.GetSection(nameof(UserServiceSettings)).Get<UserServiceSettings>();
		public virtual PostServiceSettings PostServiceSettings
			=> _configuration.GetSection(nameof(PostServiceSettings)).Get<PostServiceSettings>();

		public SettingsProviderService(IConfiguration configuration)
		{
			this._configuration = configuration;
		}
	}
}
