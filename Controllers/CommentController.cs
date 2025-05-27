using BlogApp.Data;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController(DatabaseContext context, ICommentsService commentsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetComments()
    {
      var comments = await commentsService.GetCommentsAsync();
      return Ok(comments); 
    }
    
    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> CreateComment([FromBody] CommentDto commentDto)
    {
        if(ModelState.IsValid == false) return BadRequest(ModelState);
        var comment = await commentsService.CreateCommentAsync(commentDto);
        return Ok(comment);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentResponseDto>> GetCommentById(int id)
    {
       var comment = await commentsService.GetCommentsByIdAsync(id);
       if (comment == null) return NotFound();
       return Ok(comment);
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult<CommentResponseDto>> UpdateComment(int id, [FromBody] UpdateCommentDto request)
    {
        if(ModelState.IsValid == false) return BadRequest(ModelState);
        var comment = await commentsService.UpdateCommentAsync(id, request);
        if (comment == null) return NotFound();
        
        return Ok(new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            PostId = comment.PostId,
            UserId = comment.UserId,
            Username = comment.User.Username,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            IsEdited = comment.IsEdited
        });
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<CommentResponseDto>> DeleteComment(int id)
    {
        var deleted = await commentsService.DeleteCommentAsync(id);
        if (deleted == false) return NotFound();
        return NoContent();
    }
}