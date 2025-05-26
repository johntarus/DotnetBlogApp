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

    public Task<Comment> CreateCommentAsync(Comment comment)
    {
        throw new NotImplementedException();
    }

    public async Task<Comment> UpdateCommentAsync(Comment comment)
    {
        context.Update(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public Task DeleteCommentAsync(Comment comment)
    {
        throw new NotImplementedException();
    }
}