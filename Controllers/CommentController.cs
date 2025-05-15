using BlogApp.Data;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private readonly DatabaseContext _context;
    public CommentController(DatabaseContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult> GetComments()
    {
        var comments = await _context.Comments.Include(c => c.Post)
            .Include(c => c.User)
            .Select(c=> new CommentResponseDto
            {
                Id = c.Id,
                Content = c.Content,
                PostId = c.PostId,
                UserId = c.UserId,
                Username = c.User.Username,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();
        return Ok(comments);
    }
    
    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> CreateComment([FromBody] CommentDto request)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (ModelState.IsValid == false) return BadRequest(ModelState);
        if(post == null) return NotFound("Post not found");
        if(user == null) return NotFound("User not found");
        var comment = new Comment
        {
            Content = request.Content,
            PostId = request.PostId,
            UserId = request.UserId,
            CreatedAt = DateTime.Now
        };
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        var comentResponse = new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            PostId = comment.PostId,
            UserId = comment.UserId,
            Username = user.Username,
            CreatedAt = comment.CreatedAt
        };
        return CreatedAtAction(nameof(GetCommentById), new {id = comment.Id}, comentResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentResponseDto>> GetCommentById(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        return Ok(comment);
    }
}