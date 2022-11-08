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
using Moq.Protected;
using System.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace backend.Tests.Services
{
	public class JwtServiceTests
	{
		public Mock<SettingsProviderService> defaultSettingsProviderMock;
		public JwtService defaultJwtService;
		public JwtServiceSettings defaultSettings;

		public JwtServiceTests()
		{
			defaultSettings = new JwtServiceSettings
			{
				issuer = "testIssuer",
				signingKey = long.MaxValue.ToString(),
				tokenLifespanDays = 365
			};

			defaultSettingsProviderMock = new(null);
			defaultSettingsProviderMock.SetupGet(x=>x.JwtServiceSettings).Returns(defaultSettings);

			defaultJwtService = new JwtService(defaultSettingsProviderMock.Object);
		}


		[Fact]
		public void CanCreateInstance()
		{
			Assert.NotNull(defaultJwtService);
		}


		[Fact]
		public void CanCreateAndValidate()
		{
			var service = defaultJwtService;

			Assert.NotNull(service);

			var token = service.GenerateJwtToken(new Claim[]
			{
				new Claim("username","qwerty"),
				new Claim("userrole","admin")
			});

			var decoded = service.ValidateJwtToken(token);

			Assert.NotNull(decoded);
			Assert.Equal("qwerty", decoded.FindFirst("username").Value);
			Assert.Equal("admin", decoded.FindFirst("userrole").Value);
		}

		[Fact]
		public void CannotValidateInvalidToken()
		{
			var service = defaultJwtService;

			Assert.NotNull(service);

			var token = "myrandomtoken.AndTheSignPart.AndJustSomethingElse";
			Assert.Throws<System.ArgumentException>(() => service.ValidateJwtToken(token));
		}
	}
}
