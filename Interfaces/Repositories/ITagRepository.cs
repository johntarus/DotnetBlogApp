using BlogApp.Entities;
using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Repositories;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetTags();
    Task<Tag> GetTagById(int id);
    Task<Tag> AddTag(Tag tag);
    Task<Tag> UpdateTag(Tag tag);
    Task DeleteTag(Tag tag);
}