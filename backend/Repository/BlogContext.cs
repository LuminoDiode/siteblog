using backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository
{
	public class BlogContext:DbContext
	{
		public BlogContext()
			:base()
		{
			
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>().HasAlternateKey(u => u.Email);

			modelBuilder.Entity<Post>()
				.HasGeneratedTsVectorColumn(p => p.TitleVector, "simple", p => p.Title + ' ' + p.Subtile)
				.HasIndex(p => p.TitleVector).HasMethod("GIN"); 
			modelBuilder.Entity<Post>()
				.HasGeneratedTsVectorColumn(p => p.TextVector, "simple", p => p.TextHTML)
				.HasIndex(p => p.TextVector).HasMethod("GIN"); 
			
			modelBuilder.Entity<Service>()
				.HasGeneratedTsVectorColumn(s => s.NameVector, "simple", p => p.Name)
				.HasIndex(s => s.NameVector).HasMethod("GIN");
			modelBuilder.Entity<Service>()
				.HasGeneratedTsVectorColumn(s => s.DescriptionVector, "simple", p => p.Description!)
				.HasIndex(s => s.DescriptionVector).HasMethod("GIN");

			modelBuilder.Entity<Post>().HasOne<Image>(p => p.TopImage).WithOne().OnDelete(DeleteBehavior.Cascade);
		}

		public DbSet<Image> Images { get; set; } = null!;
		public DbSet<Post> Posts { get; set; } = null!;
		public DbSet<Service> Services { get; set; } = null!;
		public DbSet<User> Users { get; set; } = null!;
	}
}
