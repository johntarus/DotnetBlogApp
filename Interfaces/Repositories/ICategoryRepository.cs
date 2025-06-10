using BlogApp.Dtos.PagedFilters;
using BlogApp.Entities;
using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<PaginatedList<Category>> GetCategoriesAsync(CategoryPagedRequest request);
    Task<Category> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Category category);
}