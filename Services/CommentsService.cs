using BlogApp.Interfaces;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;

namespace BlogApp.Services;

public class CommentsService(ICommentRepository commentRepository) : ICommentsService
{

    public async Task<List<CommentResponseDto>> GetCommentsAsync()
    {
        var comments = await commentRepository.GetCommentsAsync();
        return comments.Select(c => new CommentResponseDto
        {
            Id = c.Id,
            Content = c.Content,
            PostId = c.PostId,
            UserId = c.UserId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            IsEdited = c.IsEdited,
            Username = c.User.Username,
        }).ToList();
    }

    public async Task<CommentResponseDto> GetCommentsByIdAsync(int id)
    {
        var comment = await commentRepository.GetCommentByIdAsync(id);
        if (comment == null) return null;
        return new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            PostId = comment.PostId,
            UserId = comment.UserId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            IsEdited = comment.IsEdited,
            Username = comment.User.Username,
        };
    }
}