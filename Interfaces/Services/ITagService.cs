using BlogApp.Entities;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Services;

public interface ITagService
{
    Task<IEnumerable<TagResponseDto>> GetAllTags();
    Task<TagResponseDto> GetTagById(int id);
    Task<TagResponseDto> AddTag(AddTagDto addTag);
    Task<TagResponseDto> UpdateTag(int id, UpdateTagDto tag);
    Task<bool> DeleteTag(int id);
}