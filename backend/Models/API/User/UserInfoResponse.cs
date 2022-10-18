namespace backend.Models.API.User
{
	public class UserInfoResponse
	{
		public DateTime CreatedDate { get; set; }
		public int Id { get; set; }
		public string? Name { get; set; }
		public int[]? PostsId { get; set; }
	}
}
