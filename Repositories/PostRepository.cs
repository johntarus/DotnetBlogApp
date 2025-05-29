using BlogApp.Data;
using BlogApp.Interfaces.Repositories;
using BlogApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class PostRepository(DatabaseContext context) : IPostRepository
{

    public async Task<IEnumerable<Post>> GetPostsAsync()
    {
       return await context.Posts.Include(p=>p.User)
           .Include(p=>p.Categories)
           .Include(p=>p.Comments)
           .Include(p=>p.Likes)
           .Include(p=>p.Tags)
           .ToListAsync();
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        return await context.Posts
            .Include(p => p.User)
            .Include(p => p.Categories)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        context.Add(post);
        await context.SaveChangesAsync();
        return post;
    }

    public Task<Post> UpdatePostAsync(Post post)
    {
        throw new NotImplementedException();
    }

    public Task DeletePostAsync(Post post)
    {
        throw new NotImplementedException();
    }
}