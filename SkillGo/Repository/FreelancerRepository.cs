using Microsoft.EntityFrameworkCore;
using SkillGo.Data;
using SkillGo.Repository.IRepository;

namespace SkillGo.Repository;

public class FreelancerRepository : IFreelancerRepository
{
    private readonly ApplicationDbContext _db;

    public FreelancerRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<FreelancerProfile>> SearchAsync(string? q, int? categoryId)
    {
        q = (q ?? "").Trim();

        var query = _db.FreelancerProfiles
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x =>
                x.Title.Contains(q) ||
                (x.Bio != null && x.Bio.Contains(q)) ||
                (x.User != null && x.User.UserName != null && x.User.UserName.Contains(q)));
        }

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        return await query
            .OrderBy(x => x.Title)
            .ToListAsync();
    }

    public Task<List<FreelancerProfile>> GetByCategoryAsync(int categoryId)
        => SearchAsync(null, categoryId);

    public async Task<FreelancerProfile?> GetByIdAsync(int id)
    {
        return await _db.FreelancerProfiles
            .Include(x => x.User)
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<FreelancerProfile?> GetByUserIdAsync(string userId)
    {
        return await _db.FreelancerProfiles
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<FreelancerProfile> UpsertAsync(FreelancerProfile profile)
    {
        var existing = await _db.FreelancerProfiles
            .FirstOrDefaultAsync(x => x.UserId == profile.UserId);

        if (existing is null)
        {
            _db.FreelancerProfiles.Add(profile);
            await _db.SaveChangesAsync();
            return profile;
        }

        existing.Title = profile.Title;
        existing.Bio = profile.Bio;
        existing.HourlyRate = profile.HourlyRate;
        existing.CategoryId = profile.CategoryId;

        await _db.SaveChangesAsync();
        return existing;
    }
}