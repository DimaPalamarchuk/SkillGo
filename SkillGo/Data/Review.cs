using System.ComponentModel.DataAnnotations;

namespace SkillGo.Data;

public class Review
{
    public int Id { get; set; }

    [Required]
    public int FreelancerProfileId { get; set; }
    public FreelancerProfile? FreelancerProfile { get; set; }

    [Required]
    public string AuthorUserId { get; set; } = default!;
    public ApplicationUser? AuthorUser { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}