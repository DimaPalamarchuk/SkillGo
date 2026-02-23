using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SkillGo.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<FreelancerProfile> FreelancerProfiles => Set<FreelancerProfile>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Programming" },
            new Category { Id = 2, Name = "Design" },
            new Category { Id = 3, Name = "Computer Graphic" }
        );

        builder.Entity<UserProfile>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserProfile>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        builder.Entity<FreelancerProfile>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FreelancerProfile>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        builder.Entity<FreelancerProfile>()
            .Property(f => f.HourlyRate)
            .HasPrecision(18, 2);

        builder.Entity<FreelancerProfile>()
            .HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Review>()
            .HasOne(r => r.FreelancerProfile)
            .WithMany(f => f.Reviews)
            .HasForeignKey(r => r.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasOne(r => r.AuthorUser)
            .WithMany()
            .HasForeignKey(r => r.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}