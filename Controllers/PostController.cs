using System.Security.Claims;
using BlogApp.Data;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]

[Route("api/posts")]
public class PostController(DatabaseContext context, IPostService postService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var posts = await postService.GetPostsAsync(userId);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog(AddPostDto postDto)
    { 
        if(ModelState.IsValid == false) return BadRequest(ModelState);
       var post = await postService.CreatePostAsync(postDto);
       return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        var post = await postService.GetPostByIdAsync(id);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePost(Guid id, UpdatePostDto updatePostDto)
    {
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        var post = await postService.UpdatePostAsync(id, updatePostDto);
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
