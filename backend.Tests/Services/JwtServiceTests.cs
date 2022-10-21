using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using backend.Models.Database;
using backend.Models.Runtime;
using backend.Services;
using Moq;
using System.Security.Claims;

namespace backend.Tests.Services
{
	public class JwtServiceTests
	{
		[Fact]
		public void CanCreateInstance()
		{
			Mock<JwtServiceSettingsProvider> settingsProvider = new(null);
			settingsProvider.SetupGet(x => x.signingKey).Returns("OVERRIDE_ME");
			settingsProvider.SetupGet(x => x.tokenLifespanDays).Returns(360);
			Mock<SettingsProviderService> settingService = new(null);
			settingService.SetupGet(x => x.JwtServiceSettings).Returns(settingsProvider.Object);

			var service = new JwtService(settingService.Object);

			Assert.NotNull(service);
		}


		[Fact]
		public async void CanCreateAndValidate()
		{
			Mock<JwtServiceSettingsProvider> settingsProvider = new(null);
			settingsProvider.SetupGet(x => x.signingKey).Returns("OVERRIDE_MEOVERRIDE_ME");
			settingsProvider.SetupGet(x => x.tokenLifespanDays).Returns(360);
			Mock<SettingsProviderService> settingService = new(null);
			settingService.SetupGet(x => x.JwtServiceSettings).Returns(settingsProvider.Object);

			var service = new JwtService(settingService.Object);

			var token = service.GenerateJwtToken(new Claim[]
			{
				new Claim("username","qwerty"),
				new Claim("userrole","admin")
			});
			var decoded = await service.ValidateJwtToken(token);
			Assert.NotNull(decoded);

			Assert.Equal("qwerty", (string)(decoded!["username"]));
			Assert.Equal("admin", (string)(decoded!["userrole"]));
		}

		[Fact]
		public void CannotValidateInvalidToken()
		{
			Mock<JwtServiceSettingsProvider> settingsProvider = new(null);
			settingsProvider.SetupGet(x => x.signingKey).Returns("OVERRIDE_ME");
			settingsProvider.SetupGet(x => x.tokenLifespanDays).Returns(360);
			Mock<SettingsProviderService> settingService = new(null);
			settingService.SetupGet(x => x.JwtServiceSettings).Returns(settingsProvider.Object);

			var service = new JwtService(settingService.Object);

			var decoded = service.ValidateJwtToken("myrandomtoken.AndTheSignPart.AndJustSomethingElse").Result;
			Assert.Null(decoded);
		}
	}
}
