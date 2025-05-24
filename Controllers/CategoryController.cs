using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using BlogApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[Route("api/categories")]
public class CategoryController(DatabaseContext context, ICategoryRepository categoryRepository, ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await categoryRepository.GetCategoriesAsync();
        return Ok(categories);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateCategory(AddCategoryDto categoryDto)
    {
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        var createCategory = new Category()
        {
            Name = categoryDto.Name
        };
        context.Categories.Add(createCategory);
        await context.SaveChangesAsync();
        return Ok(createCategory);
    }

    [HttpGet("{id}")]
    public async Task<CategoryResponseDto> GetCategoryById(int id)
    {
        var category = await categoryService.GetCategoryById(id);
        return category;
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategory)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
            
        }
        if(updateCategory.Name != null)
            category.Name = updateCategory.Name;
        await context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await context.Categories.FindAsync((id));
        if (category == null)
        {
            return NotFound();
        }
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return NoContent();   
    }
}