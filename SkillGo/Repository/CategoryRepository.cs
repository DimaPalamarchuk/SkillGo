using Microsoft.EntityFrameworkCore;
using SkillGo.Data;
using SkillGo.Repository.IRepository;

namespace SkillGo.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Category>> GetAllAsync()
        => await _db.Categories.AsNoTracking().ToListAsync();

    public async Task<Category?> GetAsync(int id)
        => await _db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Category> CreateAsync(Category obj)
    {
        _db.Categories.Add(obj);
        await _db.SaveChangesAsync();
        return obj;
    }

    public async Task<Category> UpdateAsync(Category obj)
    {
        var fromDb = await _db.Categories.FirstOrDefaultAsync(x => x.Id == obj.Id);
        if (fromDb is null) return obj;

        fromDb.Name = obj.Name;
        await _db.SaveChangesAsync();
        return fromDb;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var obj = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (obj is null) return false;

        _db.Categories.Remove(obj);
        return (await _db.SaveChangesAsync()) > 0;
    }
}