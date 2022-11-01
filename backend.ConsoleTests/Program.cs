using backend.Models.Database;
using backend.Repository;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;

namespace backend.ConsoleTests
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//Mock<JwtServiceSettingsProvider> jwtSettingsProvider = new(null);
			//jwtSettingsProvider.SetupGet(x => x.signingKey).Returns("OVERRIDE_MEOVERRIDE_ME");
			//jwtSettingsProvider.SetupGet(x => x.tokenLifespanDays).Returns(360);


			//Mock<EmailConfirmationServiceSettingsProvider> emailConfirmSettingsProvider = new(null);
			//emailConfirmSettingsProvider.SetupGet(x => x.ownDomain).Returns(@"mysite.com/");
			//emailConfirmSettingsProvider.SetupGet(x => x.linkLifespanDays).Returns(1);
			//emailConfirmSettingsProvider.SetupGet(x => x.urlPathBeforeToken).Returns(@"/api/emailConfirmation/");

			//Mock<SettingsProviderService> _settingsProviderService = new(null);


			//_settingsProviderService.SetupGet(x => x.JwtServiceSettings).Returns(jwtSettingsProvider.Object);
			//_settingsProviderService.SetupGet(x => x.EmailConfirmationServiceSettings).Returns(emailConfirmSettingsProvider.Object);

			//var service = new EmailConfirmationService(_settingsProviderService.Object, new JwtService(_settingsProviderService.Object),null,null);

			//var link = service.CreateLinkForEmail("testemail@gmail.com");
			//var parsed =  service.GetEmailFromLinkIfValid(link);
			////Console.ReadKey();

			//var _tokenHandler = new JwtSecurityTokenHandler();
			//var token = _tokenHandler.WriteToken(_tokenHandler.CreateToken(new SecurityTokenDescriptor
			//{
			//	Subject = new ClaimsIdentity(new Claim[] { new Claim("username", "bidlo") }),
			//	Expires = DateTime.UtcNow.AddSeconds(1),
			//	SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("OVERRIDEMEOVERRIDEME")), SecurityAlgorithms.HmacSha512Signature)
			//}));
			//Console.WriteLine("waitin");
			//Task.Delay(1000).Wait();
			//Console.WriteLine("validating");
			//var result = _tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
			//{
			//	ValidateIssuer = false,
			//	ValidateAudience = false,
			//	ValidateLifetime = true,
			//	ValidateIssuerSigningKey = true,
			//	IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("OVERRIDEMEOVERRIDEME")),
			//	ClockSkew = TimeSpan.Zero
			//}).Result;
			//for(int i = 0; i < 1000; i++)
			//{
			//	 result = _tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
			//	{
			//		ValidateIssuer = false,
			//		ValidateAudience = false,
			//		ValidateLifetime = true,
			//		ValidateIssuerSigningKey = true,
			//		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("OVERRIDEMEOVERRIDEME")),
			//		 ClockSkew = TimeSpan.Zero
			//	 }).Result;
			//	Console.WriteLine(result.IsValid);
			//	Task.Delay(1000).Wait();
			//}

			DraftService<PostDraft,BlogContext> t = new(new BlogContext(), null, x => x.PostDrafts, x => true, x => DateTime.UtcNow);
			Console.ReadKey();
		}
	}
}