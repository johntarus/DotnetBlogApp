using AutoMapper;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : ICategoryService
{
    public async Task<PaginatedList<CategoryResponseDto>> GetCategoriesAsync([FromQuery]int pageNumber, [FromQuery]int pageSize)
    {
        var paginatedCategories = await categoryRepository.GetCategoriesAsync(pageNumber, pageSize);
        var categories =  mapper.Map<List<CategoryResponseDto>>(paginatedCategories.Items);
        return new PaginatedList<CategoryResponseDto>(categories, paginatedCategories.PageNumber, 
            paginatedCategories.PageSize, paginatedCategories.TotalCount, paginatedCategories.TotalPages);
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