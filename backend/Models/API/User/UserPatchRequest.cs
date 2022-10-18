namespace backend.Models.API.User
{
	public class UserPatchRequest
	{
		public string? NewName { get; set; }
		public string? NewPassword { get; set; }
	}
}
