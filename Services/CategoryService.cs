using AutoMapper;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : ICategoryService
{
    public async Task<List<CategoryResponseDto>> GetCategoriesAsync()
    {
        var categories = await categoryRepository.GetCategoriesAsync();

        return mapper.Map<List<CategoryResponseDto>>(categories);
    }


    public async Task<CategoryResponseDto?> GetCategoryById(int id)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id);
        if (category == null) return null;
        return mapper.Map<CategoryResponseDto>(category);
        
    }

    public async Task<CategoryResponseDto> AddCategoryAsync(AddCategoryDto categoryDto)
    {
        var category = mapper.Map<Category>(categoryDto);
        var createdCategory = await categoryRepository.CreateCategoryAsync(category);
        return mapper.Map<CategoryResponseDto>(createdCategory);
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id);
        if (category == null)
            return null;
        mapper.Map(categoryDto, category);
        var updatedCategory = await categoryRepository.UpdateCategoryAsync(category);
        return mapper.Map<CategoryResponseDto>(updatedCategory);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category =await categoryRepository.GetCategoryByIdAsync(id);
        if (category == null)
            return false;
        await categoryRepository.DeleteCategoryAsync(category);
        return true;
    }
}