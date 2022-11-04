using backend.Models.Runtime;
using backend.Services;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit.Abstractions;

namespace backend.Tests.Services
{
	public class EmailConfirmationServiceTests
	{
		ITestOutputHelper _output;
		ILogger<EmailConfirmationService> _logger;

		public Mock<SettingsProviderService> defaultSettingsProviderMock;
		public EmailConfirmationService defaultEmailConfirmationService;
		public EmailConfirmationServiceSettings defaultSettings;
		public const string userEmailAddressConst = "userEmailAddressConst@gmail.com";

		public EmailConfirmationServiceTests(ITestOutputHelper output)
		{
			_output = output;
			_logger = output.BuildLoggerFor<EmailConfirmationService>();

			defaultSettings = new EmailConfirmationServiceSettings
			{
				linkLifespanDays = 365,
				ownDomain = "https://mysite.com/",
				urlPathBeforeToken = "confirmEmail"
			};

			defaultSettingsProviderMock = new(null);
			defaultSettingsProviderMock.SetupGet(x => x.EmailConfirmationServiceSettings).Returns(defaultSettings);

			defaultEmailConfirmationService = new(defaultSettingsProviderMock.Object, new JwtServiceTests().defaultJwtService, null, _logger);
		}

		[Fact]
		public void CanCreateInstance()
		{
			Assert.NotNull(defaultEmailConfirmationService);
		}

		[Fact]
		public void CanCreateLink()
		{
			var service = defaultEmailConfirmationService;

			var link = service.CreateLinkForEmail(userEmailAddressConst);
			var withoutJwt = link.Substring(0, link.LastIndexOf('/'));
			Assert.Equal(defaultSettings.ownDomain + defaultSettings.urlPathBeforeToken, withoutJwt);
		}

		[Fact]
		public void CanValidateLink()
		{
			var service = defaultEmailConfirmationService;

			var link = service.CreateLinkForEmail(userEmailAddressConst);
			var parsed = service.GetEmailFromLinkIfValid(link);
			Assert.Equal(userEmailAddressConst, parsed);
		}

		[Fact]
		public async void CannotValidateExpired()
		{
			var mySettings = new EmailConfirmationServiceSettings
			{
				linkLifespanDays = 2.225E-6, // 200 ms, but Unix time step is 1s
				ownDomain = "https://mysite.com/",
				urlPathBeforeToken = userEmailAddressConst
			};
			defaultSettingsProviderMock.SetupGet(x => x.EmailConfirmationServiceSettings).Returns(mySettings);

			var service = new EmailConfirmationService(defaultSettingsProviderMock.Object,new JwtServiceTests().defaultJwtService,null,_logger);

			var link = service.CreateLinkForEmail(userEmailAddressConst);
			await Task.Delay(1000);
			Assert.Throws<Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException>(() => service.GetEmailFromLinkIfValid(link));
		}
	}
}
