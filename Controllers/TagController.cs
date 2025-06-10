using BlogApp.Dtos.PagedFilters;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/tags")]
public class TagController(ITagService tagService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetTags([FromQuery] TagPagedRequest request)
    {
        if(request.PageNumber <= 0 || request.PageSize <= 0) return BadRequest("Page Number and Page Size must be greater than zero");
        var tags = await tagService.GetAllTags(request);
        return Ok(tags);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] AddTagDto tagDto)
    {
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        var tag = await tagService.AddTag(tagDto);
        return Ok(tag);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagDto updateTag)
    {
        if(ModelState.IsValid == false) return BadRequest(ModelState);
        var tag = await tagService.UpdateTag(id, updateTag);
        if (tag == null) return NotFound();
        
        return Ok(tag); 
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById(int id)
    {
        var tag = await tagService.GetTagById(id);
        if (tag==null) return NotFound();
        {
            
        }
        return Ok(tag);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var deleted = await tagService.DeleteTag(id);
        if (deleted == false)
        {
            return NotFound();
        }
        return NoContent(); 
    }
}