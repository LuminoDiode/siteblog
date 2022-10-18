namespace backend.Models.API.Post
{
    public class PostInfoResponse
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string? TopImageLink { get; set; }

        public string Title { get; set; } = null!;
        public string? Subtile { get; set; }

        public int OwnerUserId { get; set; }
        public string? OwnerUserName { get; set; }
    }
}
