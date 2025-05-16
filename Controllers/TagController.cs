using BlogApp.Data;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[Route("api/tags")]
public class TagController(DatabaseContext context) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var tags = await context.Tags
            .Include(t => t.Posts)
            .Select(t => new TagResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            CreateAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            Posts = t.Posts.Select(p => new PostResponseDto
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
        return Ok(tags);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] AddTagDto tagDto)
    {
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        var createTag = new Tag()
        {
            Name = tagDto.Name,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
            await context.Tags.AddAsync(createTag);
            await context.SaveChangesAsync();
            return Ok(createTag);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagDto updateTag)
    {
        var tag = await context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound();
        }
        if(updateTag.Name != null)
            tag.Name = updateTag.Name;
        tag.UpdatedAt = DateTime.Now;
        await context.SaveChangesAsync();
        return Ok(tag);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById(int id)
    {
        var tag = await context.Tags.Where(t=>t.Id == id).Include(t=>t.Posts).Select(t=> new TagResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Posts = t.Posts.Select(p=> new PostResponseDto
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
                Tags = p.Tags.Select(t => t.Name).ToList(),
            }).ToList()
        }).ToListAsync();
        if (tag == null)
        {
            return NotFound();
        }
        return Ok(tag);  
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = await context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound();
        }
        context.Tags.Remove(tag);
        await context.SaveChangesAsync();
        return NoContent();   
    }
}