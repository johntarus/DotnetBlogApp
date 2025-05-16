using BlogApp.Data;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[Route("api/tags")]
public class TagController : Controller
{
    private readonly DatabaseContext _context;
    
    public TagController(DatabaseContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var tags = await _context.Tags
            .Select(t => new TagResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Posts = t.Posts.ToList()
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
            Name = tagDto.Name
        };
            await _context.Tags.AddAsync(createTag);
            await _context.SaveChangesAsync();
            return Ok(createTag);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagDto updateTag)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound();
        }
        if(updateTag.Name != null)
            tag.Name = updateTag.Name;
        await _context.SaveChangesAsync();
        return Ok(tag);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound();
        }
        return Ok(tag);  
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return NotFound();
        }
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return NoContent();   
    }
}