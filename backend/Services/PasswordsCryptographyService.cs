using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using backend.Models.Database;
using backend.Models.Runtime;
using backend.Services;

namespace backend.Services
{
	// https://github.com/LuminoDiode/LuminoDiodeWebsite/blob/master/Website/Services/PasswordsService.cs
	public class PasswordsCryptographyService
	{
		protected readonly PasswordsCryptographyServiceSettingsProvider _settings;
		protected readonly Func<byte[], byte[]> HashData = SHA512.HashData;
		protected readonly Func<string, byte[]> GetBytes = Encoding.UTF8.GetBytes;
		protected int SaltSizeBytes => this._settings.saltSizeBytes;

		public PasswordsCryptographyService(SettingsProviderService SettingsProvider)
		{
			this._settings = SettingsProvider.PasswordsCryptographyServiceSettings;
		}

		public byte[] HashPassword(string PlainTextPassword, out byte[] GeneratedSalt)
		{
			var Salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
			var PasswordBytes = GetBytes(PlainTextPassword);

			var SaltedPassword = PasswordBytes.Concat(Salt).ToArray();

			GeneratedSalt = Salt;
			return this.HashData(SaltedPassword); // should better use PBKDF2 ?
		}
		public bool ConfirmPassword(string PlainTextPassword, byte[] HashedPassword, byte[] Salt)
		{
			var PasswordBytes = GetBytes(PlainTextPassword);

			var SaltedPossiblePassword = PasswordBytes.Concat(Salt).ToArray();

			return this.HashData(SaltedPossiblePassword).SequenceEqual(HashedPassword);
		}
	}
}