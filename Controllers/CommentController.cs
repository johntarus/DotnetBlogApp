using BlogApp.Data;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Authorize]
[Route("api/comments")]
public class CommentController(DatabaseContext context, ICommentsService commentsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetComments([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
      var comments = await commentsService.GetCommentsAsync(pageNumber, pageSize);
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

        return Ok(comment);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<CommentResponseDto>> DeleteComment(int id)
    {
        var deleted = await commentsService.DeleteCommentAsync(id);
        if (deleted == false) return NotFound();
        return NoContent();
    }
}