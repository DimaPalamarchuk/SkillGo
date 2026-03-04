using Microsoft.EntityFrameworkCore;
using SkillGo.Data;
using SkillGo.Data.Models;
using SkillGo.Repository.IRepository;

namespace SkillGo.Repository
{
    public class WalletRepository : IWalletRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public WalletRepository(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<decimal> GetBalanceAsync(string userId)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Users
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync();
        }

        public async Task<List<WalletTransaction>> GetTransactionsAsync(string userId, int take = 30)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            return await db.WalletTransactions
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task TopUpAsync(string userId, decimal amount, string? note)
        {
            if (amount <= 0) return;

            await using var db = await _dbFactory.CreateDbContextAsync();
            await using var tx = await db.Database.BeginTransactionAsync();

            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return;

            user.Balance += amount;

            db.WalletTransactions.Add(new WalletTransaction
            {
                UserId = userId,
                Amount = amount,
                Type = WalletTransactionType.TopUp,
                Note = string.IsNullOrWhiteSpace(note) ? null : note,
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }
    }
}