using BlogApp.Data;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[ApiController]
[Route("api/likes")]
public class LikesController(DatabaseContext context, ILikeService likeService, ILogger<LikesController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLikes()
    {
        var likes = await likeService.GetLikesAsync();
        return Ok(likes);
    }
    
    [HttpPost("like")]
    public async Task<IActionResult> AddLike([FromBody] LikeDto likeDto)
    {
        try
        {
            var existingLike = await context.Likes
                .FirstOrDefaultAsync(l => l.PostId == likeDto.PostId && l.UserId == likeDto.UserId);
            
            var user = await context.Users.FindAsync(likeDto.UserId);

            if (existingLike != null)
                return Conflict("User already liked this post.");

            var like = new Like()
            {
                PostId = likeDto.PostId,
                UserId = likeDto.UserId
            };

            context.Likes.Add(like);
            await context.SaveChangesAsync();
            return Ok(new LikeResponseDto
            {
                Id = like.Id,
                PostId = like.PostId,
                UserId = like.UserId,
                Username = user.Username,
                CreatedAt = like.CreateAt
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding like");
            return StatusCode(500, "An error occurred while adding like");
        }
    }

    [HttpDelete("unlike")]
    public async Task<IActionResult> RemoveLike([FromBody] LikeDto request)
    {
        try
        {
            var like = await context.Likes.FirstOrDefaultAsync(l => l.PostId == request.PostId && l.UserId == request.UserId);
            if(like == null) return NotFound("User has not liked this post");
            context.Likes.Remove(like);
            await context.SaveChangesAsync();
            return Ok("Post unliked successfully");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error removing like");
            return StatusCode(500, "An error occurred while removing like");
        }
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckIfUserLikedPost([FromBody] CheckLikeRequestDto request)

    {
        try
        {
            var hasLiked = await context.Likes.AnyAsync(l => l.PostId == request.PostId && l.UserId == request.UserId);
            return Ok(new { hasLiked });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error checking if user liked post");
            return StatusCode(500, "An error occurred while checking if user liked post");
        }
    }

}