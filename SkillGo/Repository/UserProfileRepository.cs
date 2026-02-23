using Microsoft.EntityFrameworkCore;
using SkillGo.Data;
using SkillGo.Repository.IRepository;

namespace SkillGo.Repository;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _db;

    public UserProfileRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserProfile?> GetByUserIdAsync(string userId)
    {
        return await _db.UserProfiles
            .AsNoTracking()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<UserProfile> UpsertAsync(UserProfile profile)
    {
        var existing = await _db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == profile.UserId);

        if (existing is null)
        {
            _db.UserProfiles.Add(profile);
            await _db.SaveChangesAsync();
            return profile;
        }

        existing.DisplayName = profile.DisplayName;
        existing.About = profile.About;

        await _db.SaveChangesAsync();
        return existing;
    }
}