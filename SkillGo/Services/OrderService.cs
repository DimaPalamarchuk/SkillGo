using Microsoft.EntityFrameworkCore;
using SkillGo.Data;
using SkillGo.Data.Models;
using SkillGo.Data.Models.Chat;
using SkillGo.Data.Models.Orders;

namespace SkillGo.Services;

public class CreateOrderResult
{
    public bool Success { get; set; }
    public bool NeedTopUp { get; set; }
    public decimal MissingAmount { get; set; }
    public string? Error { get; set; }
    public int? ConversationId { get; set; }
    public int? OrderId { get; set; }
}

public class OrderService
{
    private readonly ApplicationDbContext _db;

    public OrderService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CreateOrderResult> CreateOrderAsync(int serviceId, string buyerId)
    {
        if (string.IsNullOrWhiteSpace(buyerId))
            return new CreateOrderResult { Success = false, Error = "Unauthorized" };

        var service = await _db.ServiceOffers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == serviceId);

        if (service == null)
            return new CreateOrderResult { Success = false, Error = "Service not found" };

        if (service.OwnerUserId == buyerId)
            return new CreateOrderResult { Success = false, Error = "You can't order your own service" };

        var buyer = await _db.Users.FirstOrDefaultAsync(x => x.Id == buyerId);
        if (buyer == null)
            return new CreateOrderResult { Success = false, Error = "User not found" };

        var price = service.Price;
        if (price <= 0)
            return new CreateOrderResult { Success = false, Error = "Invalid service price" };

        if (buyer.Balance < price)
        {
            return new CreateOrderResult
            {
                Success = false,
                NeedTopUp = true,
                MissingAmount = Math.Max(0, price - buyer.Balance),
                Error = "Not enough balance"
            };
        }

        var sellerId = service.OwnerUserId;

        var a = buyerId;
        var b = sellerId;
        if (string.CompareOrdinal(a, b) > 0)
        {
            var t = a;
            a = b;
            b = t;
        }

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var conversation = await _db.Conversations.FirstOrDefaultAsync(x => x.UserAId == a && x.UserBId == b);

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    UserAId = a,
                    UserBId = b
                };

                _db.Conversations.Add(conversation);
                await _db.SaveChangesAsync();
            }

            buyer.Balance -= price;

            _db.WalletTransactions.Add(new WalletTransaction
            {
                UserId = buyerId,
                Amount = price,
                Type = WalletTransactionType.Debit,
                Note = $"Order payment for service #{service.Id}"
            });

            var order = new Order
            {
                ServiceOfferId = service.Id,
                ConversationId = conversation.Id,
                BuyerId = buyerId,
                SellerId = sellerId,
                Amount = price,
                EscrowAmount = price,
                Status = OrderStatus.InProgress,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = buyerId,
                Body = null,
                OrderId = order.Id,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            return new CreateOrderResult
            {
                Success = true,
                ConversationId = conversation.Id,
                OrderId = order.Id
            };
        }
        catch
        {
            await tx.RollbackAsync();
            return new CreateOrderResult { Success = false, Error = "Failed to create order" };
        }
    }
}