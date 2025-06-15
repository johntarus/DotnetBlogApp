using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<PaginatedList<Category>> GetCategoriesAsync(CategoryPagedRequest request);
    Task<Category> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Category category);
}