using System.ComponentModel.DataAnnotations;

namespace SkillGo.Data;

public class UserProfile
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = default!;

    public ApplicationUser? User { get; set; }

    [Required, MaxLength(60)]
    public string DisplayName { get; set; } = "";

    [MaxLength(500)]
    public string? About { get; set; }
}