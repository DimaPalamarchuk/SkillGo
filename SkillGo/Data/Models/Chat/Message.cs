using System.ComponentModel.DataAnnotations;
using SkillGo.Data.Models.Orders;

namespace SkillGo.Data.Models.Chat;

public class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } = default!;

    [Required]
    public string SenderId { get; set; } = "";

    [MaxLength(4000)]
    public string? Body { get; set; }

    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? EditedAtUtc { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    public List<MessageAttachment> Attachments { get; set; } = new();
}