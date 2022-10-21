namespace backend.Models.Runtime
{
	/* Данный класс используется только для nameof. */
	public abstract class EmailConfirmationServiceSettings
	{
		public virtual string ownDomain { get; set; } = null!;
		[Obsolete]
		public virtual string delimiterString { get; set; } = null!;
		public virtual int linkLifespanDays { get; set; }
		public virtual string urlPathBeforeToken { get; set; } = null!;
	}
}
