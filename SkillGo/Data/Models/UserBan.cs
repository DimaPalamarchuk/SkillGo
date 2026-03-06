using System.ComponentModel.DataAnnotations;

namespace SkillGo.Data.Models
{
    public class UserBan
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        [Required]
        public string CreatedByUserId { get; set; } = default!;
        public ApplicationUser CreatedByUser { get; set; } = default!;

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = default!;

        public DateTime StartsAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? EndsAtUtc { get; set; }

        public bool IsActive => StartsAtUtc <= DateTime.UtcNow && (!EndsAtUtc.HasValue || EndsAtUtc > DateTime.UtcNow);
    }
}