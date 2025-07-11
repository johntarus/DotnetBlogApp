using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Utils;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Repositories;

public class CategoryRepository(DatabaseContext context) : ICategoryRepository
{
    public async Task<PaginatedList<Category>> GetCategoriesAsync(CategoryPagedRequest request)
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
        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            query = query.Where(c=>c.Name.Contains(request.SearchQuery.ToLower()));
        }

        if (request.Id.HasValue)
        {
            query = query.Where(c=>c.Id == request.Id);
        }
        return await PaginationUtils.CreateAsync(query, request.PageNumber, request.PageSize);
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