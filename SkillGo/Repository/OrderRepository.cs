using Microsoft.EntityFrameworkCore;
using SkillGo.Data;
using SkillGo.Data.Models;
using SkillGo.Data.Models.Orders;
using SkillGo.Repository.IRepository;

namespace SkillGo.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Order?> GetAsync(int id)
            => await _db.Orders
                .Include(x => x.ServiceOffer)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<Order>> GetByConversationAsync(int conversationId)
            => await _db.Orders
                .Where(x => x.ConversationId == conversationId)
                .Include(x => x.ServiceOffer)
                .OrderByDescending(x => x.CreatedAtUtc)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Order>> GetForUserAsync(string userId)
            => await _db.Orders
                .Where(x => x.BuyerId == userId || x.SellerId == userId)
                .Include(x => x.ServiceOffer)
                .OrderByDescending(x => x.CreatedAtUtc)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> CompleteAsync(int orderId, string buyerId)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();

            var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.InProgress)
                return false;

            if (order.BuyerId != buyerId)
                return false;

            if (order.EscrowAmount <= 0)
                return false;

            var seller = await _db.Users.FirstOrDefaultAsync(x => x.Id == order.SellerId);
            if (seller == null)
                return false;

            seller.Balance += order.EscrowAmount;

            _db.WalletTransactions.Add(new WalletTransaction
            {
                UserId = order.SellerId,
                Amount = order.EscrowAmount,
                Type = WalletTransactionType.OrderIncome,
                Note = $"Order #{order.Id}"
            });

            order.Status = OrderStatus.Completed;
            order.CompletedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
            return true;
        }
    }
}