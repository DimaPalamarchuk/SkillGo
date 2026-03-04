using Microsoft.EntityFrameworkCore;
using SkillGo.Data;
using SkillGo.Repository.IRepository;

namespace SkillGo.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public CategoryRepository(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category?> GetAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Category> CreateAsync(Category obj)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Categories.Add(obj);
        await db.SaveChangesAsync();
        return obj;
    }

    public async Task<Category> UpdateAsync(Category obj)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var fromDb = await db.Categories.FirstOrDefaultAsync(x => x.Id == obj.Id);
        if (fromDb is null) return obj;

        fromDb.Name = obj.Name;
        await db.SaveChangesAsync();
        return fromDb;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var obj = await db.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (obj is null) return false;

        db.Categories.Remove(obj);
        return (await db.SaveChangesAsync()) > 0;
    }
}