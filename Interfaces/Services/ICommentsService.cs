using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Services;

public interface ICommentsService
{
    Task<List<CommentResponseDto>> GetCommentsAsync();
    Task<CommentResponseDto> GetCommentsByIdAsync(int id);
    Task<CommentResponseDto> CreateCommentAsync(CommentDto comment);
    Task<Comment> UpdateCommentAsync(int id, UpdateCommentDto commentDto);
    Task DeleteCommentAsync(Comment comment);
}