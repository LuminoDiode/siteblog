using backend.Services;
using Moq;
using Xunit.Abstractions;

namespace backend.Tests.Services
{
	public class EmailConfirmationServiceTests
	{
		private readonly ITestOutputHelper _output;
		private Mock<SettingsProviderService> _settingsProviderService;

		public EmailConfirmationServiceTests(ITestOutputHelper output)
		{
			this._output = output;

			Mock<JwtServiceSettingsProvider> jwtSettingsProvider = new(null);
			jwtSettingsProvider.SetupGet(x => x.signingKey).Returns("OVERRIDE_MEOVERRIDE_ME");
			jwtSettingsProvider.SetupGet(x => x.tokenLifespanDays).Returns(360);


			Mock<EmailConfirmationServiceSettingsProvider> emailConfirmSettingsProvider = new(null);
			emailConfirmSettingsProvider.SetupGet(x => x.ownDomain).Returns(@"mysite.com/");
			emailConfirmSettingsProvider.SetupGet(x => x.linkLifespanDays).Returns(1);
			emailConfirmSettingsProvider.SetupGet(x => x.urlPathBeforeToken).Returns(@"/api/emailConfirmation/");

			_settingsProviderService = new(null);


			_settingsProviderService.SetupGet(x => x.JwtServiceSettings).Returns(jwtSettingsProvider.Object);
			_settingsProviderService.SetupGet(x => x.EmailConfirmationServiceSettings).Returns(emailConfirmSettingsProvider.Object);
		}

		[Fact]
		public void CanCreateInstance()
		{
			var service = new EmailConfirmationService(_settingsProviderService.Object,new JwtService(_settingsProviderService.Object),null, null);

			Assert.NotNull(service);
		}

		[Fact]
		public void CanCreateLink()
		{
			var service = new EmailConfirmationService(_settingsProviderService.Object, new JwtService(_settingsProviderService.Object), null, null);

			var link = service.CreateLinkForEmail("testemail@gmail.com");
			var withoutJwt = link.Substring(0, link.LastIndexOf('/'));
			Assert.Equal(@"https://mysite.com/api/emailConfirmation", withoutJwt);
		}

		[Fact]
		public async void CanValidateLink()
		{
			_settingsProviderService.SetupGet(x => x.JwtServiceSettings.issuer).Returns("OVERRIDE_ME");
			var service = new EmailConfirmationService(_settingsProviderService.Object, new JwtService(_settingsProviderService.Object), null, null);

			var link = service.CreateLinkForEmail("testemail@gmail.com");
			var parsed = service.GetEmailFromLinkIfValid(link);
			Assert.Equal(@"testemail@gmail.com", parsed);
		}

		[Fact]
		public async void CannotValidateExpired()
		{
			_settingsProviderService.SetupGet(x => x.EmailConfirmationServiceSettings.linkLifespanDays).Returns(2.225E-6); // returns 200 ms, but Unix time step is 1s


			var service = new EmailConfirmationService(_settingsProviderService.Object, new JwtService(_settingsProviderService.Object), null, null);
			var link = service.CreateLinkForEmail("testemail@gmail.com");
			await Task.Delay(1000);

			Assert.Throws<Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException>(()=>service.GetEmailFromLinkIfValid(link));
		}
	}
}
