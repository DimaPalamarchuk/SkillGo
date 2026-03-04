using System.ComponentModel.DataAnnotations;
using SkillGo.Data.Models.Chat;

namespace SkillGo.Data.Models.Orders;

public class Order
{
    public int Id { get; set; }

    public int ServiceOfferId { get; set; }
    public ServiceOffer ServiceOffer { get; set; } = default!;

    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } = default!;

    [Required]
    public string BuyerId { get; set; } = "";

    [Required]
    public string SellerId { get; set; } = "";

    public decimal Amount { get; set; }
    public decimal EscrowAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.InProgress;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }

    public int? ReviewId { get; set; }
    public Review? Review { get; set; }
}