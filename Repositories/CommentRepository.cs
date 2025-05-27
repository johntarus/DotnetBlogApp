using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class CommentRepository(DatabaseContext context) : ICommentRepository
{
    public async Task<List<Comment>> GetCommentsAsync()
    {
        return await context.Comments
            .Include(c => c.Post)
            .Include(c => c.User)
            .ToListAsync();
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