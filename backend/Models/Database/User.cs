namespace backend.Models.Database
{
    public class User
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int Id { get; set; }
        public string? Name { get; set; }
        public string EmailAddress { get; set; } = null!;
        public bool EmailConfirmed { get; set; } = false;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;

        public string UserRole { get; set; } = "user"; // user,admin,moderator
    }
}
