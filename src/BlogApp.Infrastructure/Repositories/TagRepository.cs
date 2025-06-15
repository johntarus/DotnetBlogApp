using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Utils;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Repositories;

public class TagRepository(DatabaseContext context) : ITagRepository
{
    public async Task<PaginatedList<Tag>> GetAllTags(TagPagedRequest request)
    {
        var query = context.Tags.Include(t => t.Posts).AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            query = query.Where(t => t.Name.Contains(request.SearchQuery));
        }

        if (request.Id.HasValue)
        {
            query = query.Where(t => t.Id == request.Id);
        }
        return await PaginationUtils.CreateAsync(query, request.PageNumber, request.PageSize);
    }

    public async Task<List<Tag>> GetTagsByIds(List<int> tagIds)
    {
        return await context.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
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