using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Services;

public interface ICategoryService
{
    Task<PaginatedList<CategoryResponseDto>> GetCategoriesAsync(CategoryPagedRequest request);
    Task<CategoryResponseDto?> GetCategoryById(int id);
    Task<CategoryResponseDto> AddCategoryAsync(AddCategoryDto categoryDto);
    Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
    Task<bool> DeleteCategoryAsync(int id);
}