using BlogApp.Data;
using BlogApp.Dtos.PagedFilters;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using BlogApp.Models.Entities;
using BlogApp.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class PostRepository(DatabaseContext context) : IPostRepository
{

    public async Task<PaginatedList<Post>> GetPostsAsync(PostPagedRequest request)
    {
       var query =  context.Posts.Include(p=>p.User)
           .Include(p=>p.Category)
           .Include(p=>p.Comments)
           .Include(p=>p.Likes)
           .Include(p=>p.Tags)
           .AsQueryable();
       if (!string.IsNullOrWhiteSpace(request.SearchQuery))
       {
           query = query.Where(p =>
               p.Title.Contains(request.SearchQuery) ||
               p.Content.Contains(request.SearchQuery) || 
               p.Slug.Contains(request.SearchQuery));
       }

       if (request.Id.HasValue)
       {
           query = query.Where(p=>p.Id == request.Id.Value);
       }

       if (request.UserId.HasValue)
       {
           query = query.Where(p=>p.UserId == request.UserId);
       }

       if (request.CategoryId.HasValue)
       {
           query = query.Where(p=>p.CategoryId == request.CategoryId);
       }
       
       return await PaginationUtils.CreateAsync(query, request.PageNumber, request.PageSize);
    }

    public async Task<PaginatedList<Post>> GetPostsByUserIdAsync(Guid userId, int pageNumber, int pageSize)
    {
        var query = context.Posts.Where(p => p.UserId == userId).Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.Tags)
            .AsQueryable();
        
        var totalItems = await context.Posts.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var posts = await query.OrderByDescending(p=>p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PaginatedList<Post>(posts, pageNumber, pageSize, totalItems, totalPages);
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        return await context.Posts
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        context.Add(post);
        await context.SaveChangesAsync();
        return await context.Posts.Include(p=>p.User).Include(p=>p.Category).FirstOrDefaultAsync(p => p.Id == post.Id);
    }

    public async Task<Post> UpdatePostAsync(Post post)
    {
        context.Update(post);
        await context.SaveChangesAsync();
        return post;
    }

    public async Task DeletePostAsync(Post post)
    {
        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }
}