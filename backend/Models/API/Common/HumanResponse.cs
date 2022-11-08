namespace backend.Models.API.Common
{
	public class HumanNotification
	{
		public const string errType = "Error";
		public const string warnType = "Warning";
		public const string succType = "Success";
		public const string infoType = "Information";

		public string HeaderText;
		public string BodyText;
		public string Type;

		public HumanNotification(string headerText, string bodyText, string type=HumanNotification.infoType)
		{
			this.HeaderText = headerText;
			this.BodyText = bodyText;
			this.Type = type;
		}	
	}
	public class HumanResponse
	{
		public HumanNotification[]? HumanNotifications { get; set; }

		public HumanResponse(params string[]? humanNotifications)
		{
			this.HumanNotifications = humanNotifications?.Select(n=> new HumanNotification("Attention!",n)).ToArray();
		}

		public HumanResponse(params HumanNotification[]? humanNotifications)
		{
			this.HumanNotifications = humanNotifications;
		}
	}
}
