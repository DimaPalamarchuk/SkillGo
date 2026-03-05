using System.ComponentModel.DataAnnotations;

namespace SkillGo.Data.Models.Orders;

public enum OrderDisputeStatus
{
    Open = 1,
    Resolved = 2,
    Rejected = 3
}

public class OrderDispute
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;

    [Required]
    public string ReporterId { get; set; } = "";

    [MaxLength(120)]
    public string Subject { get; set; } = "";

    [MaxLength(2000)]
    public string Description { get; set; } = "";

    public OrderDisputeStatus Status { get; set; } = OrderDisputeStatus.Open;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; set; }
}