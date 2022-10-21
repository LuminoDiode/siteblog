using backend.Services;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			var service = new EmailConfirmationService(_settingsProviderService.Object,new JwtService(_settingsProviderService.Object));

			Assert.NotNull(service);
		}

		[Fact]
		public void CanCreateLink()
		{
			var service = new EmailConfirmationService(_settingsProviderService.Object, new JwtService(_settingsProviderService.Object));

			var link = service.CreateLinkForEmail("testemail@gmail.com");
			_output.WriteLine($"generated link = \'{link}\'");
			_output.WriteLine($"the jwt will be \'{link.Split('/').Last()}\'");
			var withoutJwt = link.Substring(0, link.LastIndexOf('/'));
			Assert.Equal(@"http://mysite.com/api/emailConfirmation", withoutJwt);
		}

		[Fact]
		public async void CanValidateLink()
		{
			var service = new EmailConfirmationService(_settingsProviderService.Object, new JwtService(_settingsProviderService.Object));

			var link = service.CreateLinkForEmail("testemail@gmail.com");
			var parsed = await service.GetInfoFromLink(link);
			Assert.NotNull(parsed);
			Assert.Equal(@"testemail@gmail.com", parsed!);
		}

		[Fact]
		public async void CannotValidateExpired()
		{
			_settingsProviderService.SetupGet(x => x.EmailConfirmationServiceSettings.linkLifespanDays).Returns(2.225E-6); // 200 ms
			_output.WriteLine((DateTime.UtcNow.AddDays(_settingsProviderService.Object.EmailConfirmationServiceSettings.linkLifespanDays) - DateTime.UtcNow)
				.Milliseconds.ToString());
			var service = new EmailConfirmationService(_settingsProviderService.Object, new JwtService(_settingsProviderService.Object));

			var link = service.CreateLinkForEmail("testemail@gmail.com");
			await Task.Delay(3000);
			Assert.Null(await service.GetInfoFromLink(link));
		}
	}
}
