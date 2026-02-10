using SkillGo.Data;

namespace SkillGo.Repository.IRepository
{
    public interface ICategoryRepository
    {
        public Task<Category> CreateAsync(Category obj);
        public Task<Category> UpdateAsync(Category obj);
        public Task<bool> DeleteAsync(int id);
        public Task<IEnumerable<Category>> GetAllAsync();
        public Task<Category> GetAsync(int id);
    }
}
