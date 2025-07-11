using System.Text.Json;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers;

[ApiController]
[Authorize]
[Route("api/likes")]
public class LikesController(ILikeService likeService, ILogger<LikesController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLikes([FromQuery] LikesPagedRequest request)
    {
        logger.LogInformation("Getting likes with request: {Request}", JsonSerializer.Serialize(request));
        if (request.PageNumber < 1 || request.PageSize < 1)
        {
            logger.LogWarning("Invalid pagination parameters.");
            return BadRequest("Page Number and Page Size must be greater than or equal to 1.");
        }

        var result = await likeService.GetLikesAsync(request);
        logger.LogInformation("Likes fetched: {Response}", JsonSerializer.Serialize(result));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetLikeById(int id)
    {
        logger.LogInformation("Getting like by ID: {Id}", id);
        var like = await likeService.GetLikeByIdAsync(id);
        if (like == null)
        {
            logger.LogWarning("Like with ID {Id} not found.", id);
            return NotFound();
        }

        logger.LogInformation("Like fetched: {Response}", JsonSerializer.Serialize(like));
        return Ok(like);
    }

    [HttpPost("like")]
    public async Task<IActionResult> AddLike([FromBody] LikeDto dto)
    {
        logger.LogInformation("Add like request: {Dto}", JsonSerializer.Serialize(dto));
        try
        {
            var createdLike = await likeService.CreateLikeAsync(dto);
            if (createdLike == null)
            {
                logger.LogWarning("Like already exists.");
                return Conflict("Already liked");
            }

            logger.LogInformation("Like created: {Response}", JsonSerializer.Serialize(createdLike));
            return Ok(createdLike);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating like.");
            return StatusCode(500, "An error occurred while creating the like.");
        }
    }

    [HttpDelete("unlike")]
    public async Task<IActionResult> RemoveLike([FromBody] LikeDto dto)
    {
        logger.LogInformation("Remove like request: {Dto}", JsonSerializer.Serialize(dto));
        var removed = await likeService.RemoveLikeAsync(dto);
        logger.LogInformation("Like removed: {Status}", removed);
        return removed ? Ok("Post unliked") : NotFound("Like not found");
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckIfUserLikedPost([FromBody] CheckLikeRequestDto dto)
    {
        logger.LogInformation("Check like request: {Dto}", JsonSerializer.Serialize(dto));
        var hasLiked = await likeService.CheckIfLikedAsync(dto);
        logger.LogInformation("User has liked post: {HasLiked}", hasLiked);
        return Ok(new { hasLiked });
    }
}