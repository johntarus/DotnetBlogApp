using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using BlogApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[Route("api/categories")]
public class CategoryController(DatabaseContext context, ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await categoryService.GetCategoriesAsync();
        return Ok(categories);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateCategory(AddCategoryDto categoryDto)
    {
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        var category = await categoryService.AddCategoryAsync(categoryDto);
        return Ok(category);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await categoryService.GetCategoryById(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategory)
    {
        if(ModelState.IsValid == false) return BadRequest(ModelState);
        var category = await categoryService.UpdateCategoryAsync(id, updateCategory);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);  
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var deleted = await categoryService.DeleteCategoryAsync(id);
        if (deleted == false)
        {
            return NotFound();
        }
        return NoContent(); 
    }
}