using SkillGo.Data.Models.Orders;

namespace SkillGo.Services;

public class MessageDto
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public string SenderId { get; set; } = "";
    public string? Body { get; set; }
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? EditedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}