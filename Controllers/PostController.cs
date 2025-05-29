using BlogApp.Data;
using BlogApp.Helpers;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Entities;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[ApiController]

[Route("api/posts")]
public class PostController(DatabaseContext context, IPostService postService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await postService.GetPostsAsync();
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog(AddPostDto postDto)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == postDto.CategoryId);
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == postDto.UserId);
        if(category == null)
            return NotFound("Category not found");
        if(user == null)
            return NotFound("User not found");
        var post = new Post()
        {
            Title = postDto.Title,
            Content = postDto.Content,
            Slug = SlugUtils.GenerateSlug(postDto.Title),
            CategoryId = postDto.CategoryId,
            UserId = postDto.UserId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        if (postDto.TagIds != null && postDto.TagIds.Any())
        {
            var tags = await context.Tags.Where(t => postDto.TagIds.Contains(t.Id)).ToListAsync();
            post.Tags = tags;
        }
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        var postDtoResponse = new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            Content = post.Content,
            CategoryId = post.CategoryId,
            CategoryName = category.Name,
            UserId = user.Id,
            Username = user.Username,
            CreatedAt = post.CreatedAt
        };
        return CreatedAtAction(nameof(GetPostById), new {id = post.Id}, postDtoResponse);
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
        var post = await context.Posts.Include(p=>p.User)
            .Include(p=>p.Categories).FirstOrDefaultAsync(p => p.Id == id);
        if (post == null)
        {
            return NotFound();
        }
        
        if(updatePostDto.Title != null)
            post.Title = updatePostDto.Title;
        if(updatePostDto.Content != null)
            post.Content = updatePostDto.Content;
        post.UpdatedAt = DateTime.Now;
        
            await context.SaveChangesAsync();
            return Ok(new PostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                CategoryId = post.CategoryId,
                CategoryName = post.Categories.Name,
                UserId = post.UserId,
                Username = post.User.Username,
            });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(Guid id)
    {
        var blog = await context.Posts.FindAsync(id);
        if (blog == null)
        {
            return NotFound();
        }
        context.Posts.Remove(blog);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
