using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class LikeRepository(DatabaseContext context) : ILikeRepository
{
    public async Task<IEnumerable<Like>> GetLikesAsync()
    {
        return await context.Likes.Include(l=>l.User).ToListAsync();
    }

    public async Task<Like> GetLikeByIdAsync(int id)
    {
        return await context.Likes.Include(l=>l.User).FirstOrDefaultAsync(l => l.Id == id);
    }

    public Task<Like> CreateLikeAsync(Like like)
    {
        throw new NotImplementedException();
    }

    public Task<Like> UpdateLikeAsync(Like like)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteLikeAsync(Like like)
    {
        throw new NotImplementedException();
    }
}