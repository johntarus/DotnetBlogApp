using BlogApp.Entities;
using BlogApp.Models.Entities;

namespace BlogApp.Interfaces;

public interface ICommentRepository
{
    Task<PaginatedList<Comment>> GetCommentsAsync(int pageNumber, int pageSize);
    Task<Comment> GetCommentByIdAsync(int id);
    Task<Comment> CreateCommentAsync(Comment comment);
    Task<Comment> UpdateCommentAsync(Comment comment);
    Task DeleteCommentAsync(Comment comment);
}