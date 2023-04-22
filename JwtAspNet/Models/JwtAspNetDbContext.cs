using Microsoft.EntityFrameworkCore;

namespace JwtAspNet.Models
{
    public class JwtAspNetDbContext : DbContext
    {
        public JwtAspNetDbContext()
        {

        }

        public JwtAspNetDbContext(DbContextOptions<JwtAspNetDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(e => e.Email)
                .IsUnique();
        }

        public DbSet<User> Users { get; set; }
    }
}
