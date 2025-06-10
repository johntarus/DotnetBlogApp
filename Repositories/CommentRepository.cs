using BlogApp.Data;
using BlogApp.Entities;
using BlogApp.Interfaces;
using BlogApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class CommentRepository(DatabaseContext context) : ICommentRepository
{
    public async Task<PaginatedList<Comment>> GetCommentsAsync(int pageNumber, int pageSize)
    {
        var query = context.Comments
            .Include(c => c.Post)
            .Include(c => c.User)
            .AsQueryable();
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var comments = await query.OrderByDescending(p=>p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PaginatedList<Comment>(comments, pageNumber, pageSize, totalCount, totalPages);
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