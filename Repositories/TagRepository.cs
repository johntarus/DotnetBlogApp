using BlogApp.Data;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class TagRepository(DatabaseContext context) : ITagRepository
{
    public async Task<IEnumerable<Tag>> GetAllTags()
    {
        return await context.Tags.Include(t=>t.Posts).ToListAsync();
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