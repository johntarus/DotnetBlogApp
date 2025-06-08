using System.Security.Claims;
using BlogApp.Dtos.Request;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]

[Route("api/posts")]
public class PostController(IPostService postService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        Console.WriteLine($"user Role: {userRole}");
        var isAdmin = userRole?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
        var posts = await postService.GetPostsAsync(userId, isAdmin);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog(AddPostDto postDto)
    { 
        if(ModelState.IsValid == false) return BadRequest(ModelState);
        // var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("Invalid user identification. Please sign in again.");
        }
        var post = await postService.CreatePostAsync(postDto, userId);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        var post = await postService.GetPostByIdAsync(id);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePost(Guid id, UpdatePostDto updatePostDto)
    {
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(userClaim == null || !Guid.TryParse(userClaim.Value, out Guid userId))
            return Unauthorized("Invalid user identification. Please sign in again.");
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        var post = await postService.UpdatePostAsync(id, updatePostDto, userId, roleClaim.Value);
        return Ok(post);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(Guid id)
    {
        var isPostDeleted = await postService.DeletePostAsync(id);
        if(isPostDeleted == false) return NotFound();
        return NoContent();
    }
}
