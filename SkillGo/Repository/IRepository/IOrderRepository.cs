using SkillGo.Data.Models.Orders;

namespace SkillGo.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<Order?> GetAsync(int id);
        Task<List<Order>> GetByConversationAsync(int conversationId);
        Task<List<Order>> GetForUserAsync(string userId);
        Task<bool> CompleteAsync(int orderId, string buyerId);
    }
}