using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface ICommentsService
{
    Task<List<CommentResponseDto>> GetCommentsAsync();
    Task<CommentResponseDto> GetCommentsByIdAsync(int id);
}