using AutoMapper;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Services;

public class CommentsService(ICommentRepository commentRepository, IMapper mapper, ILogger<CommentsService> logger)
    : ICommentsService
{
    public async Task<PaginatedList<CommentResponseDto>> GetCommentsAsync(CommentPagedRequest request)
    {
        logger.LogInformation("Service: Retrieving comments with PageNumber: {PageNumber}, PageSize: {PageSize}", request.PageNumber, request.PageSize);

        var paginatedComments = await commentRepository.GetCommentsAsync(request);
        var comments = mapper.Map<List<CommentResponseDto>>(paginatedComments.Items);

        logger.LogInformation("Service: Retrieved {Count} comments", comments.Count);
        return new PaginatedList<CommentResponseDto>(
            comments,
            paginatedComments.PageNumber,
            paginatedComments.PageSize,
            paginatedComments.TotalCount,
            paginatedComments.TotalPages
        );
    }

    public async Task<CommentResponseDto> CreateCommentAsync(CommentDto commentDto)
    {
        logger.LogInformation("Service: Creating new comment for PostId: {PostId}", commentDto.PostId);

        var comment = mapper.Map<Comment>(commentDto);
        var createdComment = await commentRepository.CreateCommentAsync(comment);

        logger.LogInformation("Service: Comment created with Id: {Id}", createdComment.Id);
        return mapper.Map<CommentResponseDto>(createdComment);
    }

    public async Task<CommentResponseDto> GetCommentsByIdAsync(int id)
    {
        logger.LogInformation("Service: Fetching comment with Id: {Id}", id);

        var comment = await commentRepository.GetCommentByIdAsync(id);
        if (comment == null)
        {
            logger.LogWarning("Service: Comment with Id: {Id} not found", id);
            return null;
        }

        logger.LogInformation("Service: Comment with Id: {Id} retrieved", id);
        return mapper.Map<CommentResponseDto>(comment);
    }

    public async Task<CommentResponseDto> UpdateCommentAsync(int id, UpdateCommentDto commentDto)
    {
        logger.LogInformation("Service: Updating comment with Id: {Id}", id);

        var commentToUpdate = await commentRepository.GetCommentByIdAsync(id);
        if (commentToUpdate == null)
        {
            logger.LogWarning("Service: Comment with Id: {Id} not found for update", id);
            return null;
        }

        mapper.Map(commentDto, commentToUpdate);
        commentToUpdate.UpdatedAt = DateTime.UtcNow;
        commentToUpdate.IsEdited = true;

        var updatedComment = await commentRepository.UpdateCommentAsync(commentToUpdate);

        logger.LogInformation("Service: Comment with Id: {Id} updated", id);
        return mapper.Map<CommentResponseDto>(updatedComment);
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        logger.LogInformation("Service: Deleting comment with Id: {Id}", id);

        var comment = await commentRepository.GetCommentByIdAsync(id);
        if (comment == null)
        {
            logger.LogWarning("Service: Comment with Id: {Id} not found for deletion", id);
            return false;
        }

        await commentRepository.DeleteCommentAsync(comment);
        logger.LogInformation("Service: Comment with Id: {Id} deleted", id);
        return true;
    }
}
