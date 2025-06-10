using AutoMapper;
using BlogApp.Dtos.PagedFilters;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger) : ICategoryService
{
    public async Task<PaginatedList<CategoryResponseDto>> GetCategoriesAsync(CategoryPagedRequest request)
    {
        logger.LogInformation("Service call: GetCategoriesAsync with PageNumber: {PageNumber}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        var paginatedCategories = await categoryRepository.GetCategoriesAsync(request);
        var categoriesDto = mapper.Map<List<CategoryResponseDto>>(paginatedCategories.Items);

        logger.LogInformation("Mapping completed. Returning {Count} categories", categoriesDto.Count);

        return new PaginatedList<CategoryResponseDto>(
            categoriesDto,
            paginatedCategories.PageNumber,
            paginatedCategories.PageSize,
            paginatedCategories.TotalCount,
            paginatedCategories.TotalPages
        );
    }

public async Task<CategoryResponseDto?> GetCategoryById(int id)
{
    logger.LogDebug("Service layer - Fetching category with ID: {CategoryId}", id);
    
    var category = await categoryRepository.GetCategoryByIdAsync(id);
    if (category == null)
    {
        logger.LogDebug("Service layer - Category with ID {CategoryId} not found", id);
        return null;
    }
    
    var result = mapper.Map<CategoryResponseDto>(category);
    logger.LogDebug("Service layer - Retrieved category data: {@Category}", result);
    
    return result;
}

public async Task<CategoryResponseDto> AddCategoryAsync(AddCategoryDto categoryDto)
{
    logger.LogDebug("Service layer - Adding new category with name: {CategoryName}", categoryDto.Name);
    
    var category = mapper.Map<Category>(categoryDto);
    var createdCategory = await categoryRepository.CreateCategoryAsync(category);
    var result = mapper.Map<CategoryResponseDto>(createdCategory);
    
    logger.LogDebug("Service layer - Created category with ID: {CategoryId}", result.Id);
    return result;
}

public async Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
{
    logger.LogDebug("Service layer - Updating category with ID: {CategoryId}", id);
    
    var category = await categoryRepository.GetCategoryByIdAsync(id);
    if (category == null)
    {
        logger.LogDebug("Service layer - Category with ID {CategoryId} not found for update", id);
        return null;
    }
    
    mapper.Map(categoryDto, category);
    var updatedCategory = await categoryRepository.UpdateCategoryAsync(category);
    var result = mapper.Map<CategoryResponseDto>(updatedCategory);
    
    logger.LogDebug("Service layer - Updated category with ID: {CategoryId}", id);
    return result;
}

public async Task<bool> DeleteCategoryAsync(int id)
{
    logger.LogDebug("Service layer - Deleting category with ID: {CategoryId}", id);
    
    var category = await categoryRepository.GetCategoryByIdAsync(id);
    if (category == null)
    {
        logger.LogDebug("Service layer - Category with ID {CategoryId} not found for deletion", id);
        return false;
    }
    
    await categoryRepository.DeleteCategoryAsync(category);
    logger.LogDebug("Service layer - Deleted category with ID: {CategoryId}", id);
    return true;
}
}