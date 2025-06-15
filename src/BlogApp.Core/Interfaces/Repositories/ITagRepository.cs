using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Repositories;

public interface ITagRepository
{
    Task<PaginatedList<Tag>> GetAllTags(TagPagedRequest request);
    Task<List<Tag>> GetTagsByIds(List<int> tagIds);
    Task<Tag> GetTagById(int id);
    Task<Tag> AddTag(Tag tag);
    Task<Tag> UpdateTag(Tag tag);
    Task DeleteTag(Tag tag);
}