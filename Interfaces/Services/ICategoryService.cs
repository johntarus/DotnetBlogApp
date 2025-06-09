using BlogApp.Entities;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Services;

public interface ICategoryService
{
    Task<PaginatedList<CategoryResponseDto>> GetCategoriesAsync(int pageNumber, int pageSize);
    Task<CategoryResponseDto?> GetCategoryById(int id);
    Task<CategoryResponseDto> AddCategoryAsync(AddCategoryDto categoryDto);
    Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
    Task<bool> DeleteCategoryAsync(int id);
}