namespace backend.Models.API.User
{
	public class UserPatchRequest
	{
		public int? Id { get; set; }
		public string? NewName { get; set; }
		public string? NewPassword { get; set; }
	}
}
