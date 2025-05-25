using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace BlogApp.Repositories;

public class CategoryRepository(DatabaseContext context) : ICategoryRepository
{
    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await context.Categories
            .Include(c => c.Posts)
            .ThenInclude(p => p.User)
            .Include(c => c.Posts)
            .ThenInclude(p => p.Tags)
            .Include(c => c.Posts)
            .ThenInclude(p => p.Likes)
            .Include(c => c.Posts)
            .ThenInclude(p => p.Comments)
            .ToListAsync();
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

    public Task<Category> CreateCategoryAsync(Category category)
    {
        throw new NotImplementedException();
    }

    public Task<Category> UpdateCategoryAsync(Category category)
    {
        throw new NotImplementedException();
    }

    public Task<Category> DeleteCategoryAsync(int id)
    {
        throw new NotImplementedException();
    }
}