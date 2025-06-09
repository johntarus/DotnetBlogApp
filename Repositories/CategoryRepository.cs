using BlogApp.Data;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using BlogApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace BlogApp.Repositories;

public class CategoryRepository(DatabaseContext context) : ICategoryRepository
{
    public async Task<PaginatedList<Category>> GetCategoriesAsync(int pageNumber, int pageSize)
    {
        var query = context.Categories
            .Include(c => c.Posts)
            .ThenInclude(p => p.User)
            .Include(c => c.Posts)
            .ThenInclude(p => p.Tags)
            .Include(c => c.Posts)
            .ThenInclude(p => p.Likes)
            .Include(c => c.Posts)
            .ThenInclude(p => p.Comments)
            .AsQueryable();
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var posts = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<Category>(posts, pageNumber, pageSize, totalCount, totalPages);
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        var category = await context.Categories.Where(c => c.Id == id)
            .Include(c => c.Posts)
            .ThenInclude(p => p.User)
            .Include(c => c.Posts)
            .ThenInclude(p => p.Tags)
            .Include(c => c.Posts)
            .ThenInclude(p => p.Likes)
            .Include(c => c.Posts)
            .ThenInclude(p => p.Comments)
            .FirstOrDefaultAsync();
        return category;
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        context.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        context.Update(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategoryAsync(Category category)
    {
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }
}