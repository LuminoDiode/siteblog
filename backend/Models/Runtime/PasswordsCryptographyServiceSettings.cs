namespace backend.Models.Runtime
{
	public class PasswordsCryptographyServiceSettings
	{
		public virtual int saltSizeBytes { get; set; } = 128;
	}
}
