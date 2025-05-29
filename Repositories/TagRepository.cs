using BlogApp.Data;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using BlogApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class TagRepository(DatabaseContext context) : ITagRepository
{
    public async Task<IEnumerable<Tag>> GetTags()
    {
        return await context.Tags.Include(t=>t.Posts).ToListAsync();
    }

    public async Task<Tag> GetTagById(int id)
    {
      var tag = await context.Tags.Include(t=>t.Posts).FirstOrDefaultAsync(t => t.Id == id);
      return tag;
    }

    public Task<Tag> AddTag(Tag tag)
    {
        throw new NotImplementedException();
    }

    public Task<Tag> UpdateTag(Tag tag)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTag(Tag tag)
    {
        throw new NotImplementedException();
    }
}