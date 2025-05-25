using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetCategoriesAsync();
    Task<CategoryResponseDto?> GetCategoryById(int id);
    Task<Category> AddCategoryAsync(AddCategoryDto categoryDto);
    Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
    Task<bool> DeleteCategoryAsync(int id);
}