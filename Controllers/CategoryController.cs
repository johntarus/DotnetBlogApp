using BlogApp.Data;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly DatabaseContext _context;

    public CategoryController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Posts = c.Posts.Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Categories.Name,
                    UserId = p.UserId,
                    Username = p.User.Username,
                    LikesCount = p.Likes.Count,
                    CommentsCount = p.Comments.Count,
                    Tags = p.Tags.Select(t => t.Name).ToList()
                }).ToList()
            })
            .ToListAsync();

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
        _context.Categories.Add(createCategory);
        await _context.SaveChangesAsync();
        return Ok(createCategory);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _context.Categories.Where(c=>c.Id == id)
            .Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Posts = c.Posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Categories.Name,
                UserId = p.UserId,
                Username = p.User.Username,
                LikesCount = p.Likes.Count,
                CommentsCount = p.Comments.Count,
                Tags = p.Tags.Select(t => t.Name).ToList()
            }).ToList()
        }).ToListAsync();
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);   
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategory)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
            
        }
        if(updateCategory.Name != null)
            category.Name = updateCategory.Name;
        await _context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync((id));
        if (category == null)
        {
            return NotFound();
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();   
    }
}