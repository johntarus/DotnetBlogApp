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
        var tags = await _context.Tags.ToListAsync();
        return Ok(tags);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag(AddTagDto tagDto)
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
    public async Task<IActionResult> UpdateTag(int id, UpdateTagDto updateTag)
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
}