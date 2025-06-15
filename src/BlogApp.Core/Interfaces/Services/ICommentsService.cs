using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Services;

public interface ICommentsService
{
    Task<PaginatedList<CommentResponseDto>> GetCommentsAsync(CommentPagedRequest request);
    Task<CommentResponseDto> GetCommentsByIdAsync(int id);
    Task<CommentResponseDto> CreateCommentAsync(CommentDto comment);
    Task<CommentResponseDto> UpdateCommentAsync(int id, UpdateCommentDto commentDto);
    Task<bool> DeleteCommentAsync(int id);
}