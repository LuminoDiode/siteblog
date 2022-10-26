namespace backend.Models.Runtime
{
	/* Данный класс используется только для nameof. */
	public abstract class JwtServiceSettings
	{
		public virtual int tokenLifespanDays { get; set; }
		public virtual string signingKey { get; set; } = null!;
		public virtual string issuer { get; set; } = null!;
	}
}
