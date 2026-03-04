using SkillGo.Data.Models;

namespace SkillGo.Repository.IRepository
{
    public interface IWalletRepository
    {
        Task<decimal> GetBalanceAsync(string userId);
        Task<List<WalletTransaction>> GetTransactionsAsync(string userId, int take = 30);
    }
}