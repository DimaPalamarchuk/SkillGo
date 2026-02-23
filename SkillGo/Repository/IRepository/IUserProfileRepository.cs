using SkillGo.Data;

namespace SkillGo.Repository.IRepository;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(string userId);
    Task<UserProfile> UpsertAsync(UserProfile profile);
}