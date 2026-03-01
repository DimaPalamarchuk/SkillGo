using System;
using System.ComponentModel.DataAnnotations;

namespace SkillGo.Data.Models
{
    public class Report
    {
        public int Id { get; set; }

        [Required]
        public string ReporterUserId { get; set; } = default!;
        public ApplicationUser ReporterUser { get; set; } = default!;

        [Required]
        public string TargetUserId { get; set; } = default!;
        public ApplicationUser TargetUser { get; set; } = default!;

        public int? ServiceOfferId { get; set; }
        public ServiceOffer? ServiceOffer { get; set; }

        [Required, MaxLength(500)]
        public string Reason { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}