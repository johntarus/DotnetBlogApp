// PostController.cs
using System.Security.Claims;
using System.Text.Json;
using BlogApp.Dtos.PagedFilters;
using BlogApp.Dtos.Request;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Authorize]
[Route("api/posts")]
public class PostController(IPostService postService, ILogger<PostController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPosts([FromQuery] PostPagedRequest request)
    {
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            logger.LogWarning("Invalid paging parameters: {Request}", JsonSerializer.Serialize(request));
            return BadRequest("Page Number and Page Size must be greater than zero");
        }

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userId == Guid.Empty || string.IsNullOrEmpty(userRole))
        {
            logger.LogWarning("Unauthorized access attempt.");
            return Unauthorized();
        }

        var isAdmin = userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        var posts = await postService.GetPostsAsync(userId, isAdmin, request);
        logger.LogInformation("Fetched posts: {Posts}", JsonSerializer.Serialize(posts));
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog(AddPostDto postDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("Invalid user identification. Please sign in again.");
        }

        logger.LogInformation("Creating post for user {UserId} with data: {PostDto}", userId, JsonSerializer.Serialize(postDto));
        var post = await postService.CreatePostAsync(postDto, userId);
        logger.LogInformation("Created post: {Post}", JsonSerializer.Serialize(post));
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("Invalid user identification. Please sign in again.");
        }

        logger.LogInformation("Fetching post {PostId} for user {UserId}", id, userId);
        var post = await postService.GetPostByIdAsync(id, userId, role);
        if (post == null) return NotFound();
        logger.LogInformation("Fetched post: {Post}", JsonSerializer.Serialize(post));
        return Ok(post);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePost(Guid id, UpdatePostDto updatePostDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(userClaim?.Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        logger.LogInformation("Updating post {PostId} by user {UserId} with data: {UpdateDto}", id, userId, JsonSerializer.Serialize(updatePostDto));
        var post = await postService.UpdatePostAsync(id, updatePostDto, userId, role);
        logger.LogInformation("Updated post: {Post}", JsonSerializer.Serialize(post));
        return Ok(post);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(Guid id)
    {
        var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(userClaim?.Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        logger.LogInformation("User {UserId} requested to delete post {PostId}", userId, id);
        var isPostDeleted = await postService.DeletePostAsync(id, userId, role);
        logger.LogInformation("Post deleted: {Deleted}", isPostDeleted);

        if (!isPostDeleted) return NotFound();
        return NoContent();
    }
}