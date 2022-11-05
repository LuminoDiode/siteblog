namespace backend.Models.API.Post
{
	public class PostsInfoRequest
	{
		public int StartIndex { get; set; }= 0;
		public int MaxCount { get; set; } = 30;
	}
}
