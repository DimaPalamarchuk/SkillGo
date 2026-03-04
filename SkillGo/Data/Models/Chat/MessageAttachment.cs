using System.ComponentModel.DataAnnotations;

namespace SkillGo.Data.Models.Chat;

public class MessageAttachment
{
    public int Id { get; set; }

    public int MessageId { get; set; }
    public Message Message { get; set; } = default!;

    [Required, MaxLength(260)]
    public string FileName { get; set; } = "";

    [Required, MaxLength(500)]
    public string Url { get; set; } = "";

    [MaxLength(120)]
    public string? ContentType { get; set; }

    public long SizeBytes { get; set; }

    public bool IsImage { get; set; }
}