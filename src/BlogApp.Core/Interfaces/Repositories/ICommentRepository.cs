using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<PaginatedList<Comment>> GetCommentsAsync(CommentPagedRequest request);
    Task<Comment> GetCommentByIdAsync(int id);
    Task<Comment> CreateCommentAsync(Comment comment);
    Task<Comment> UpdateCommentAsync(Comment comment);
    Task DeleteCommentAsync(Comment comment);
}