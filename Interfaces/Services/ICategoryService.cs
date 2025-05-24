namespace BlogApp.Services;

public interface ICategoryService
{
    Task<CategoryResponseDto?> GetCategoryById(int id);
}