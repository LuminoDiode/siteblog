namespace backend.Models.Runtime
{
	public class RecentImagesServiceSettings
	{
		public int numberOfImagesStored { get; set; } = 18; // default
		public int renewIntervalSeconds { get; set; } = 60; // default
	}
}
