using SkillGo.Data;

namespace SkillGo.Repository.IRepository;

public interface IFreelancerRepository
{
    Task<List<FreelancerProfile>> SearchAsync(string? q, int? categoryId);
    Task<List<FreelancerProfile>> GetByCategoryAsync(int categoryId);

    Task<FreelancerProfile?> GetByIdAsync(int id);
    Task<FreelancerProfile?> GetByUserIdAsync(string userId);

    Task<FreelancerProfile> UpsertAsync(FreelancerProfile profile);
}