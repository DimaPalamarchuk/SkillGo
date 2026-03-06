using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillGo.Data.Models;
using SkillGo.Data.Models.Chat;
using SkillGo.Data.Models.Orders;
using SkillGo.Data.Models.Reports;

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

    public DbSet<ServiceOffer> ServiceOffers => Set<ServiceOffer>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Report> Reports => Set<Report>();

    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDispute> OrderDisputes => Set<OrderDispute>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Programming" },
            new Category { Id = 2, Name = "Design" },
            new Category { Id = 3, Name = "Computer Graphic" }
        );

        builder.Entity<ApplicationUser>()
            .Property(x => x.Balance)
            .HasPrecision(18, 2);

        builder.Entity<WalletTransaction>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Entity<WalletTransaction>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ServiceOffer>()
            .HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ServiceOffer>()
            .HasOne(x => x.OwnerUser)
            .WithMany()
            .HasForeignKey(x => x.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasOne(x => x.TargetUser)
            .WithMany()
            .HasForeignKey(x => x.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasOne(x => x.AuthorUser)
            .WithMany()
            .HasForeignKey(x => x.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Report>()
            .HasOne(x => x.TargetUser)
            .WithMany()
            .HasForeignKey(x => x.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Report>()
            .HasOne(x => x.ReporterUser)
            .WithMany()
            .HasForeignKey(x => x.ReporterUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Report>()
            .HasOne(x => x.ServiceOffer)
            .WithMany()
            .HasForeignKey(x => x.ServiceOfferId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Conversation>()
            .HasIndex(x => new { x.UserAId, x.UserBId })
            .IsUnique();

        builder.Entity<Conversation>()
            .HasMany(x => x.Messages)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
            .HasMany(x => x.Attachments)
            .WithOne(x => x.Message)
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
            .HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Order>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(x => x.EscrowAmount)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .HasOne(x => x.ServiceOffer)
            .WithMany()
            .HasForeignKey(x => x.ServiceOfferId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Order>()
            .HasOne(x => x.Conversation)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Order>()
            .HasOne(x => x.Review)
            .WithMany()
            .HasForeignKey(x => x.ReviewId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Order>()
            .HasIndex(x => x.ReviewId)
            .IsUnique()
            .HasFilter("[ReviewId] IS NOT NULL");

        builder.Entity<Order>()
            .HasIndex(x => x.BuyerId);

        builder.Entity<Order>()
            .HasIndex(x => x.SellerId);

        builder.Entity<OrderDispute>()
            .HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderDispute>()
            .HasIndex(x => x.OrderId);

        builder.Entity<OrderDispute>()
            .HasIndex(x => x.ReporterId);

        builder.Entity<Report>()
            .HasOne(x => x.TargetUser)
            .WithMany()
            .HasForeignKey(x => x.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Report>()
            .HasOne(x => x.ReporterUser)
            .WithMany()
            .HasForeignKey(x => x.ReporterUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Report>()
            .HasOne(x => x.ServiceOffer)
            .WithMany()
            .HasForeignKey(x => x.ServiceOfferId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Report>()
            .HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Report>()
            .HasOne(x => x.Message)
            .WithMany()
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Report>()
            .HasIndex(x => x.TargetType);

        builder.Entity<Report>()
            .HasIndex(x => x.CreatedAt);
    }
}