using System.Text.Json;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/tags")]
public class TagController(ITagService tagService, ILogger<TagController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetTags([FromQuery] TagPagedRequest request)
    {
        logger.LogInformation("[GET] GetTags called with request: {request}", JsonSerializer.Serialize(request));
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            logger.LogWarning("Invalid pagination parameters: PageNumber={PageNumber}, PageSize={PageSize}", request.PageNumber, request.PageSize);
            return BadRequest("Page Number and Page Size must be greater than zero");
        }
        var tags = await tagService.GetAllTags(request);
        logger.LogInformation("Returning {count} tags", tags.Items.Count);
        return Ok(tags);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] AddTagDto tagDto)
    {
        logger.LogInformation("[POST] CreateTag called with: {tagDto}", JsonSerializer.Serialize(tagDto));
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for CreateTag: {errors}", JsonSerializer.Serialize(ModelState));
            return BadRequest(ModelState);
        }
        var tag = await tagService.AddTag(tagDto);
        logger.LogInformation("Tag created: {tag}", JsonSerializer.Serialize(tag));
        return Ok(tag);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagDto updateTag)
    {
        logger.LogInformation("[PATCH] UpdateTag called with id={id}, updateTag={updateTag}", id, JsonSerializer.Serialize(updateTag));
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for UpdateTag: {errors}", JsonSerializer.Serialize(ModelState));
            return BadRequest(ModelState);
        }
        var tag = await tagService.UpdateTag(id, updateTag);
        if (tag == null)
        {
            logger.LogWarning("Tag with id={id} not found", id);
            return NotFound();
        }
        logger.LogInformation("Tag updated: {tag}", JsonSerializer.Serialize(tag));
        return Ok(tag);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById(int id)
    {
        logger.LogInformation("[GET] GetTagById called with id={id}", id);
        var tag = await tagService.GetTagById(id);
        if (tag == null)
        {
            logger.LogWarning("Tag with id={id} not found", id);
            return NotFound();
        }
        logger.LogInformation("Returning tag: {tag}", JsonSerializer.Serialize(tag));
        return Ok(tag);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        logger.LogInformation("[DELETE] DeleteTag called with id={id}", id);
        var deleted = await tagService.DeleteTag(id);
        if (!deleted)
        {
            logger.LogWarning("Failed to delete tag with id={id}", id);
            return NotFound();
        }
        logger.LogInformation("Tag with id={id} deleted", id);
        return NoContent();
    }
}