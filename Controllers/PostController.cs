using BlogApp.Data;
using BlogApp.Helpers;
using BlogApp.Models.Entities;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[ApiController]

[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly DatabaseContext _context;

    public PostController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _context.Posts
            .Include(p=>p.User)
            .Include(p=>p.Categories)
            .Include(p=>p.Tags)
            .Select(p=> new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Categories.Name,
                UserId = p.UserId,
                Username = p.User.Username,
                LikesCount = p.Likes.Count,
                CommentsCount = p.Comments.Count,
                Tags = p.Tags.Select(t => t.Name).ToList(),
            })
            .ToListAsync();
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog(AddPostDto postDto)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == postDto.CategoryId);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == postDto.UserId);
        if(category == null)
            return NotFound("Category not found");
        if(user == null)
            return NotFound("User not found");
        var post = new Post()
        {
            Title = postDto.Title,
            Content = postDto.Content,
            Slug = SlugHelper.GenerateSlug(postDto.Title),
            CategoryId = postDto.CategoryId,
            UserId = postDto.UserId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        if (postDto.TagIds != null && postDto.TagIds.Any())
        {
            var tags = await _context.Tags.Where(t => postDto.TagIds.Contains(t.Id)).ToListAsync();
            post.Tags = tags;
        }
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

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
        var post = await _context.Posts.Where(p => p.Id == id)
            .Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Categories.Name,
                UserId = p.UserId,
                Username = p.User.Username,
                LikesCount = p.Likes.Count,
                CommentsCount = p.Comments.Count,
                Tags = p.Tags.Select(t => t.Name).ToList(),
            })
            .FirstOrDefaultAsync();
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateBlog(Guid id, UpdatePostDto updatePostDto)
    {
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        var blog = await _context.Posts.FindAsync(id);
        if (blog == null)
        {
            return NotFound();
        }
        
        if(updatePostDto.Title != null)
            blog.Title = updatePostDto.Title;
        if(updatePostDto.Content != null)
            blog.Content = updatePostDto.Content;
        blog.UpdatedAt = DateTime.Now;
        
            await _context.SaveChangesAsync();
            return Ok(blog);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(Guid id)
    {
        var blog = await _context.Posts.FindAsync(id);
        if (blog == null)
        {
            return NotFound();
        }
        _context.Posts.Remove(blog);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
