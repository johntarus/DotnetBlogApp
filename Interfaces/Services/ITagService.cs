using BlogApp.Entities;
using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface ITagService
{
    Task<PaginatedList<TagResponseDto>> GetAllTags(int pageNumber, int pageSize);
    Task<TagResponseDto> GetTagById(int id);
    Task<TagResponseDto> AddTag(AddTagDto addTag);
    Task<TagResponseDto> UpdateTag(int id, UpdateTagDto tag);
    Task<bool> DeleteTag(int id);
}