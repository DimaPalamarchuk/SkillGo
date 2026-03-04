using Microsoft.EntityFrameworkCore;
using SkillGo.Data;
using SkillGo.Data.Models.Chat;
using SkillGo.Data.Models.Orders;

namespace SkillGo.Services;

public class OrderCreateResult
{
    public bool Success { get; set; }
    public bool NeedTopUp { get; set; }
    public decimal MissingAmount { get; set; }
    public int? ConversationId { get; set; }
    public int? OrderId { get; set; }
    public string? Error { get; set; }
}

public class OrderService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public OrderService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<OrderCreateResult> CreateOrderAsync(int serviceOfferId, string buyerId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var offer = await db.ServiceOffers.FirstOrDefaultAsync(x => x.Id == serviceOfferId);
        if (offer == null)
            return new OrderCreateResult { Success = false, Error = "Service not found" };

        var sellerId = offer.OwnerUserId;

        if (string.IsNullOrWhiteSpace(sellerId) || sellerId == buyerId)
            return new OrderCreateResult { Success = false, Error = "Invalid seller" };

        var amount = offer.Price;

        if (amount <= 0)
            return new OrderCreateResult { Success = false, Error = "Invalid price" };

        var buyer = await db.Users.FirstOrDefaultAsync(x => x.Id == buyerId);
        if (buyer == null)
            return new OrderCreateResult { Success = false, Error = "Buyer not found" };

        if (buyer.Balance < amount)
        {
            return new OrderCreateResult
            {
                Success = false,
                NeedTopUp = true,
                MissingAmount = Math.Round(amount - buyer.Balance, 2, MidpointRounding.AwayFromZero)
            };
        }

        var a = string.CompareOrdinal(buyerId, sellerId) <= 0 ? buyerId : sellerId;
        var b = a == buyerId ? sellerId : buyerId;

        var conversation = await db.Conversations.FirstOrDefaultAsync(x => x.UserAId == a && x.UserBId == b);
        if (conversation == null)
        {
            conversation = new Conversation
            {
                UserAId = a,
                UserBId = b,
                CreatedAtUtc = DateTime.UtcNow
            };
            db.Conversations.Add(conversation);
            await db.SaveChangesAsync();
        }

        buyer.Balance -= amount;

        var order = new Order
        {
            ServiceOfferId = serviceOfferId,
            ConversationId = conversation.Id,
            BuyerId = buyerId,
            SellerId = sellerId,
            Amount = amount,
            EscrowAmount = amount,
            Status = OrderStatus.InProgress,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        db.Messages.Add(new Message
        {
            ConversationId = conversation.Id,
            SenderId = buyerId,
            Body = $"Order #{order.Id} placed and paid. Funds are held by the platform.",
            CreatedAtUtc = DateTime.UtcNow
        });

        conversation.LastMessageAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return new OrderCreateResult
        {
            Success = true,
            ConversationId = conversation.Id,
            OrderId = order.Id
        };
    }
}