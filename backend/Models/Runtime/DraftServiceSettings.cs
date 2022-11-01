namespace backend.Models.Runtime
{
	public class DraftServiceSettings
	{
		public int maxEntitiesStoredInRam { get; set; }
		public double maxTimeEntityStoredInRamMinutes { get; set; }
		public int maxEntitiesStoredInDb { get; set; }
		public double maxTimeEntityStoredInDbMinutes { get; set; }

		public double updateStoredEntitiesIntervalMinutes { get; set; }
	}
}
