namespace backend.Models.Runtime
{
	public class JwtServiceSettings
	{
		public int tokenLifespanDays { get; set; }
		public string? signingKey { get; set; }
	}
}
