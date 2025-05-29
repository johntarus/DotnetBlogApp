using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Route("api/likes")]
public class LikesController(ILikeService likeService, ILogger<LikesController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLikes() => Ok(await likeService.GetLikesAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLikeById(int id)
    {
        var like = await likeService.GetLikeByIdAsync(id);
        return like == null ? NotFound() : Ok(like);
    }

    [HttpPost("like")]
    public async Task<IActionResult> AddLike([FromBody] LikeDto dto)
    {
        try
        {
            var createdLike = await likeService.CreateLikeAsync(dto);
            return createdLike == null ? Conflict("Already liked") : Ok(createdLike);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding like");
            return StatusCode(500, "An error occurred");
        }
    }

    [HttpDelete("unlike")]
    public async Task<IActionResult> RemoveLike([FromBody] LikeDto dto)
    {
        try
        {
            var removed = await likeService.RemoveLikeAsync(dto);
            return removed ? Ok("Post unliked") : NotFound("Like not found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unliking post");
            return StatusCode(500, "An error occurred");
        }
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckIfUserLikedPost([FromBody] CheckLikeRequestDto dto)
    {
        try
        {
            var hasLiked = await likeService.CheckIfLikedAsync(dto);
            return Ok(new { hasLiked });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking like");
            return StatusCode(500, "An error occurred");
        }
    }
}