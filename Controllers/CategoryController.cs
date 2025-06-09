using BlogApp.Data;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[Route("api/categories")]
public class CategoryController(DatabaseContext context, ICategoryService categoryService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCategories(int pageNumber, int pageSize)
    {
        if(pageNumber <= 0 || pageSize <= 0) return BadRequest("Page Number and Page Size must be greater than zero");
        var categories = await categoryService.GetCategoriesAsync(pageNumber, pageSize );
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