using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] CategoryPagedRequest request)
    {
        logger.LogInformation("Fetching categories with PageNumber: {PageNumber}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            logger.LogWarning("Invalid pagination parameters: PageNumber={PageNumber}, PageSize={PageSize}",
                request.PageNumber, request.PageSize);
            return BadRequest("Page Number and Page Size must be greater than zero");
        }

        try
        {
            var result = await categoryService.GetCategoriesAsync(request);

            logger.LogInformation("Successfully retrieved {Count} categories", result.Items.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching categories");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }


    [HttpPost]
    public async Task<IActionResult> CreateCategory(AddCategoryDto categoryDto)
    {
        logger.LogInformation("Creating new category with name: {CategoryName}", categoryDto.Name);

        if (ModelState.IsValid == false)
        {
            logger.LogWarning("Invalid model state for category creation. Errors: {@Errors}",
                ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(ModelState);
        }

        var category = await categoryService.AddCategoryAsync(categoryDto);
        logger.LogInformation("Successfully created category with ID: {CategoryId}", category.Id);

        return Ok(category);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        logger.LogInformation("Fetching category with ID: {CategoryId}", id);

        var category = await categoryService.GetCategoryById(id);
        if (category == null)
        {
            logger.LogWarning("Category with ID {CategoryId} not found", id);
            return NotFound();
        }

        logger.LogDebug("Retrieved category: {@Category}", category);
        return Ok(category);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategory)
    {
        logger.LogInformation("Updating category with ID: {CategoryId}", id);

        if (ModelState.IsValid == false)
        {
            logger.LogWarning("Invalid model state for category update. Errors: {@Errors}",
                ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(ModelState);
        }

        var category = await categoryService.UpdateCategoryAsync(id, updateCategory);
        if (category == null)
        {
            logger.LogWarning("Category with ID {CategoryId} not found for update", id);
            return NotFound();
        }

        logger.LogInformation("Successfully updated category with ID: {CategoryId}", id);
        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        logger.LogInformation("Deleting category with ID: {CategoryId}", id);

        var deleted = await categoryService.DeleteCategoryAsync(id);
        if (deleted == false)
        {
            logger.LogWarning("Category with ID {CategoryId} not found for deletion", id);
            return NotFound();
        }

        logger.LogInformation("Successfully deleted category with ID: {CategoryId}", id);
        return NoContent();
    }
}