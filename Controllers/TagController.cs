using BlogApp.Data;
using BlogApp.Entities;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[Route("api/tags")]
public class TagController(DatabaseContext context, ITagService tagService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var tags = await tagService.GetAllTags();
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
        var tag = await tagService.GetTagById(id);
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