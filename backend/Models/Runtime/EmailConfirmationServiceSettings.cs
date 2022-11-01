namespace backend.Models.Runtime
{
	/* Данный класс используется только для nameof. */
	public class EmailConfirmationServiceSettings
	{
		public virtual string ownDomain { get; set; } = @"https://bruhcontent.ru";
		public virtual int linkLifespanDays { get; set; } = 365;
		public virtual string urlPathBeforeToken { get; set; } = @"/emailConfirmation/";
	}
}
