using System.ComponentModel.DataAnnotations;

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

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<MessageAttachment> Attachments { get; set; } = new();
}