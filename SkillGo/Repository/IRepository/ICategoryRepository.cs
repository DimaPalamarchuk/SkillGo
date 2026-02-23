using SkillGo.Data;

namespace SkillGo.Repository.IRepository;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetAsync(int id);

    Task<Category> CreateAsync(Category obj);
    Task<Category> UpdateAsync(Category obj);
    Task<bool> DeleteAsync(int id);
}