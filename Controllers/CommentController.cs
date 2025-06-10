using BlogApp.Data;
using BlogApp.Dtos.PagedFilters;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Authorize]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentsService commentsService;
    private readonly ILogger<CommentController> logger;

    public CommentController(DatabaseContext context, ICommentsService commentsService, ILogger<CommentController> logger)
    {
        this.commentsService = commentsService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetComments([FromQuery] CommentPagedRequest request)
    {
        logger.LogInformation("Fetching comments with PageNumber: {PageNumber}, PageSize: {PageSize}", request.PageNumber, request.PageSize);

        if (request.PageNumber < 1 || request.PageSize < 1)
        {
            logger.LogWarning("Invalid pagination parameters: PageNumber={PageNumber}, PageSize={PageSize}", request.PageNumber, request.PageSize);
            return BadRequest("Page number must be greater than or equal to 1");
        }

        var comments = await commentsService.GetCommentsAsync(request);
        logger.LogInformation("Successfully fetched {Count} comments", comments.Items.Count());
        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> CreateComment([FromBody] CommentDto commentDto)
    {
        logger.LogInformation("Creating comment for PostId: {PostId} by UserId: {UserId}", commentDto.PostId, commentDto.UserId);

        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state while creating comment");
            return BadRequest(ModelState);
        }

        var comment = await commentsService.CreateCommentAsync(commentDto);
        logger.LogInformation("Comment created successfully with Id: {Id}", comment.Id);
        return Ok(comment);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentResponseDto>> GetCommentById(int id)
    {
        logger.LogInformation("Fetching comment by Id: {Id}", id);

        var comment = await commentsService.GetCommentsByIdAsync(id);
        if (comment == null)
        {
            logger.LogWarning("Comment with Id: {Id} not found", id);
            return NotFound();
        }

        logger.LogInformation("Comment with Id: {Id} fetched successfully", id);
        return Ok(comment);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<CommentResponseDto>> UpdateComment(int id, [FromBody] UpdateCommentDto request)
    {
        logger.LogInformation("Updating comment with Id: {Id}", id);

        if (!ModelState.IsValid)
        {
            logger.LogWarning("Invalid model state for updating comment with Id: {Id}", id);
            return BadRequest(ModelState);
        }

        var comment = await commentsService.UpdateCommentAsync(id, request);
        if (comment == null)
        {
            logger.LogWarning("Comment with Id: {Id} not found for update", id);
            return NotFound();
        }

        logger.LogInformation("Comment with Id: {Id} updated successfully", id);
        return Ok(comment);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        logger.LogInformation("Deleting comment with Id: {Id}", id);

        var deleted = await commentsService.DeleteCommentAsync(id);
        if (!deleted)
        {
            logger.LogWarning("Comment with Id: {Id} not found for deletion", id);
            return NotFound();
        }

        logger.LogInformation("Comment with Id: {Id} deleted successfully", id);
        return NoContent();
    }
}
