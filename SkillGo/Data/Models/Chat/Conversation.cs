using System.ComponentModel.DataAnnotations;
using SkillGo.Data.Models.Orders;

namespace SkillGo.Data.Models.Chat;

public class Conversation
{
    public int Id { get; set; }

    [Required]
    public string UserAId { get; set; } = "";

    [Required]
    public string UserBId { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAtUtc { get; set; }

    public List<Message> Messages { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
}