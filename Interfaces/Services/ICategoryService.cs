namespace BlogApp.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetCategoriesAsync();
    Task<CategoryResponseDto?> GetCategoryById(int id);
}