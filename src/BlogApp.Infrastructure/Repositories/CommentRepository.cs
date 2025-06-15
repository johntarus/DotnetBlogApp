using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Utils;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Repositories;

public class CommentRepository(DatabaseContext context) : ICommentRepository
{
    public async Task<PaginatedList<Comment>> GetCommentsAsync(CommentPagedRequest request)
    {
        var query = context.Comments
            .Include(c => c.Post)
            .Include(c => c.User)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            query = query.Where(c=>c.Content.ToLower().Contains(request.SearchQuery.ToLower()));
        }

        if (request.IsEdited.HasValue)
        {
            query = query.Where(c => c.IsEdited == request.IsEdited.Value);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(c => c.UserId == request.UserId);
        }

        if (request.PostId.HasValue)
        {
            query = query.Where(c => c.PostId == request.PostId);
        }

        return await PaginationUtils.CreateAsync(query, request.PageNumber, request.PageSize);
    }

    public async Task<Comment> GetCommentByIdAsync(int id)
    {
        return await context.Comments.Where(c => c.Id == id)
            .Include(c => c.Post)
            .Include(c => c.User)
            .FirstOrDefaultAsync();
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        context.Add(comment);
        await context.SaveChangesAsync();
        return await context.Comments.Include(c=>c.User).FirstOrDefaultAsync(c => c.Id == comment.Id);
    }

    public async Task<Comment> UpdateCommentAsync(Comment comment)
    {
        context.Update(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public async Task DeleteCommentAsync(Comment comment)
    {
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }
}