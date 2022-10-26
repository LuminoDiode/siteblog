namespace backend.Models.API.Common
{
	public class HumanResponse
	{
		public string[]? HumanNotifications { get; set; }

		public HumanResponse(params string[]? humanNotifications)
		{
			this.HumanNotifications = humanNotifications;
		}
	}
}
