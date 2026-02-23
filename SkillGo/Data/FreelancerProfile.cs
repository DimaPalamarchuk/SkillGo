using System.ComponentModel.DataAnnotations;

namespace SkillGo.Data;

public class FreelancerProfile
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = default!;

    public ApplicationUser? User { get; set; }

    [Required, MaxLength(80)]
    public string Title { get; set; } = "";

    [MaxLength(2000)]
    public string? Bio { get; set; }

    public decimal HourlyRate { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public string? AvatarUrl { get; set; }

    public double Rating { get; set; } = 0;
    public int ReviewsCount { get; set; } = 0;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}