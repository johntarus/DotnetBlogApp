using BlogApp.Data;
using BlogApp.Helpers;
using BlogApp.Models.Entities;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[Route("api/[controller]")]
[ApiController]
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
        var blogs = await _context.Blogs.ToListAsync();
        return Ok(blogs);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog(AddPostDto postDto)
    {
        var blogEntity = new Post()
        {
            Title = postDto.Title,
            Content = postDto.Content,
            Slug = SlugHelper.GenerateSlug(postDto.Title),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _context.Blogs.Add(blogEntity);
        await _context.SaveChangesAsync();
        return Ok(blogEntity);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
        {
            return NotFound();
        }
        return Ok(blog);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateBlog(Guid id, UpdatePostDto updatePostDto)
    {
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        var blog = await _context.Blogs.FindAsync(id);
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

    [HttpDelete("Id")]
    public async Task<IActionResult> DeleteBlog(Guid id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
        {
            return NotFound();
        }
        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
