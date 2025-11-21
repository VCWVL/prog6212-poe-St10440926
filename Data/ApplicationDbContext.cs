using Microsoft.EntityFrameworkCore;
using st10440926_poeparttwo.Models;

namespace st10440926_poeparttwo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ClaimModel> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USERS TABLE
            modelBuilder.Entity<UserModel>()
                .HasKey(u => u.Username);

            // CLAIMS TABLE
            modelBuilder.Entity<ClaimModel>()
                .HasKey(c => c.Id);

            // FK: Claim → User
            modelBuilder.Entity<ClaimModel>()
                .HasOne<UserModel>()
                .WithMany()
                .HasForeignKey(c => c.LecturerUsername)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
