using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace backend.Models.Runtime
{
	public class UsernameConstraints
	{
		public virtual int minLen { get; set; } = 3;
		public virtual int maxLen { get; set; } = 32;
		public virtual bool whitespaceAllowed { get; set; } = false;
		public virtual bool nonLetterOrDigitAllowed { get; set; } = false;

		public UsernameConstraints()
		{

		}
	}
	public class PasswordConstraints
	{
		public virtual int minLen { get; set; } = 6;
		public virtual int maxLen { get; set; } = 256;
		public virtual bool digitRequired { get; set; } = true;
		public virtual bool letterRequired { get; set; } = true;

		public PasswordConstraints()
		{

		}
	}
	public class UserServiceSettings
	{
		public UsernameConstraints _usernameConstraints { get; set; } =new();
		public PasswordConstraints _passwordConstraints { get; set; } = new();
	}
}
