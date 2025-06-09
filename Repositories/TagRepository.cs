using BlogApp.Data;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class TagRepository(DatabaseContext context) : ITagRepository
{
    public async Task<PaginatedList<Tag>> GetAllTags(int pageNumber, int pageSize)
    {
        var query = context.Tags.Include(t => t.Posts).AsQueryable();
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var tags = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PaginatedList<Tag>(tags, pageNumber, pageSize, totalCount, totalPages);
    }

    public async Task<Tag> GetTagById(int id)
    {
      var tag = await context.Tags.Include(t=>t.Posts).FirstOrDefaultAsync(t => t.Id == id);
      return tag;
    }

    public async Task<Tag> AddTag(Tag tag)
    {
        context.Tags.Add(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateTag(Tag tag)
    {
        context.Tags.Update(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task DeleteTag(Tag tag)
    {
        context.Tags.Remove(tag);
        await context.SaveChangesAsync();
    }
}