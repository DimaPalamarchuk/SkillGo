using System;
using System.ComponentModel.DataAnnotations;
using SkillGo.Data.Models.Chat;
using SkillGo.Data.Models.Orders;

namespace SkillGo.Data.Models.Reports
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

        [Required]
        public ReportTargetType TargetType { get; set; }

        public int? ServiceOfferId { get; set; }
        public ServiceOffer? ServiceOffer { get; set; }

        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public int? MessageId { get; set; }
        public Message? Message { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ReportStatus Status { get; set; } = ReportStatus.Open;

        public string? ResolvedByUserId { get; set; }
        public ApplicationUser? ResolvedByUser { get; set; }

        public DateTime? ResolvedAtUtc { get; set; }

        [MaxLength(120)]
        public string? ResolutionResult { get; set; }

        [MaxLength(1000)]
        public string? ResolutionNote { get; set; }
    }
}