namespace backend.Models.Runtime
{
	public class DraftServiceSettings
	{
		public int maxEntitiesStoredInRam { get; set; } = 100;
		public double maxTimeEntityStoredInRamMinutes { get; set; } = 5;
		public int maxEntitiesStoredInDb { get; set; } = (int)1e6;
		public double maxTimeEntityStoredInDbMinutes { get; set; } = 120 * 24 * 60; // 120 days (x24h x60m)

		public double updateStoredEntitiesIntervalMinutes { get; set; } = 5;
	}
}
