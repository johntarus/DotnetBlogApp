using BlogApp.Dtos.PagedFilters;
using BlogApp.Entities;
using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface ICategoryService
{
    Task<PaginatedList<CategoryResponseDto>> GetCategoriesAsync(CategoryPagedRequest request);
    Task<CategoryResponseDto?> GetCategoryById(int id);
    Task<CategoryResponseDto> AddCategoryAsync(AddCategoryDto categoryDto);
    Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
    Task<bool> DeleteCategoryAsync(int id);
}