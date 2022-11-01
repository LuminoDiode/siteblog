namespace backend.Models.Runtime
{
	/* Данный класс используется только для nameof. */
	public class JwtServiceSettings
	{
		public virtual double tokenLifespanDays { get; set; } = 365;

#pragma warning disable CS8618
		// this fields must be left null so it will throw if
		// the signing key or issuer are not set by secrets.json.
		// that prevents going to release with a default key
		public virtual string signingKey { get; set; }
		public virtual string issuer { get; set; }
#pragma warning restore CS8618
	}
}
