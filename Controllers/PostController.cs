using System.Security.Claims;
using BlogApp.Dtos.Request;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Authorize]
[Route("api/posts")]
public class PostController(IPostService postService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPosts(int pageNumber = 1, int pageSize = 5)
    {
        if(pageNumber <= 0 || pageSize <= 0) return BadRequest("Page Number and Page Size must be greater than zero");
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if(userId == null || userRole == null) return Unauthorized();
        var isAdmin = userRole?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
        var posts = await postService.GetPostsAsync(userId, isAdmin, pageNumber, pageSize);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog(AddPostDto postDto)
    { 
        if(ModelState.IsValid == false) return BadRequest(ModelState);
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("Invalid user identification. Please sign in again.");
        }
        var post = await postService.GetPostByIdAsync(id, userId, role);
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
        var userId = Guid.Parse(userClaim?.Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var post = await postService.UpdatePostAsync(id, updatePostDto, userId, role);
        return Ok(post);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(Guid id)
    {
        var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(userClaim?.Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var isPostDeleted = await postService.DeletePostAsync(id, userId, role);
        if(isPostDeleted == false) return NotFound();
        return NoContent();
    }
}
