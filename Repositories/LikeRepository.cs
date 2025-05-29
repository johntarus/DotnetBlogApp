using BlogApp.Data;
using BlogApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class LikeRepository(DatabaseContext context) : ILikeRepository
{
    public async Task<List<Like>> GetLikesAsync() =>
        await context.Likes.Include(l => l.User).ToListAsync();

    public async Task<Like?> GetLikeByIdAsync(int id) =>
        await context.Likes.Include(l => l.User).FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Like?> GetLikeAsync(Guid postId, Guid userId) =>
        await context.Likes.Include(l => l.User)
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

    public async Task<Like> AddLikeAsync(Like like)
    {
        context.Likes.Add(like);
        await context.SaveChangesAsync();
        return like;
    }

    public async Task<bool> RemoveLikeAsync(Guid postId, Guid userId)
    {
        var like = await context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
        if (like == null) return false;
        context.Likes.Remove(like);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasUserLikedPostAsync(Guid postId, Guid userId) =>
        await context.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId);
}