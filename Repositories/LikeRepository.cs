using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class LikeRepository(DatabaseContext context) : ILikeRepository
{
    public async Task<IEnumerable<Like>> GetLikesAsync()
    {
        var likes = await context.Likes.Include(l=>l.User).ToListAsync();
        return likes;
    }

    public Task<Like> GetLikeByIdAsync(int id)
    {
        throw new NotImplementedException();
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