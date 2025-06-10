using BlogApp.Dtos.PagedFilters;
using BlogApp.Entities;
using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface ICommentsService
{
    Task<PaginatedList<CommentResponseDto>> GetCommentsAsync(CommentPagedRequest request);
    Task<CommentResponseDto> GetCommentsByIdAsync(int id);
    Task<CommentResponseDto> CreateCommentAsync(CommentDto comment);
    Task<CommentResponseDto> UpdateCommentAsync(int id, UpdateCommentDto commentDto);
    Task<bool> DeleteCommentAsync(int id);
}