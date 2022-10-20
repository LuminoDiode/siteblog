namespace backend.Models.Runtime
{
	/* Данный класс используется только для nameof. */
	public abstract class EmailConfirmationServiceSettings
	{
		public virtual string ownDomain { get; set; } = null!;
		public virtual string delimiterString { get; set; } = null!;
		public virtual string linkLifespanDays { get; set; } = null!;
	}
}
