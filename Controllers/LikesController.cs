using BlogApp.Dtos.Request;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Authorize]
[Route("api/likes")]
public class LikesController(ILikeService likeService, ILogger<LikesController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLikes([FromQuery] PagedRequestDto request)
    {
        if(request.PageNumber < 1 || request.PageSize < 1) return BadRequest("Page Number and Page Size must be greater than or equal to 1.");
        return Ok(await likeService.GetLikesAsync(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLikeById(int id)
    {
        var like = await likeService.GetLikeByIdAsync(id);
        return like == null ? NotFound() : Ok(like);
    }

    [HttpPost("like")]
    public async Task<IActionResult> AddLike([FromBody] LikeDto dto)
    {
            var createdLike = await likeService.CreateLikeAsync(dto);
            return createdLike == null ? Conflict("Already liked") : Ok(createdLike);
    }

    [HttpDelete("unlike")]
    public async Task<IActionResult> RemoveLike([FromBody] LikeDto dto)
    {
            var removed = await likeService.RemoveLikeAsync(dto);
            return removed ? Ok("Post unliked") : NotFound("Like not found");
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckIfUserLikedPost([FromBody] CheckLikeRequestDto dto)
    {
            var hasLiked = await likeService.CheckIfLikedAsync(dto);
            return Ok(new { hasLiked });
    }
}