using BlogApp.Interfaces;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;

namespace BlogApp.Services;

public class LikeService(ILikeRepository likeRepository) : ILikeService
{
    public async Task<List<LikeResponseDto>> GetLikesAsync()
    {
       var likes = await likeRepository.GetLikesAsync();
       return likes.Select(l => new LikeResponseDto()
       {
           Id = l.Id,
           UserId = l.UserId,
           PostId = l.PostId,
           Username = l.User.Username,
           CreatedAt = l.CreateAt
       }).ToList();
    }

    public Task<LikeResponseDto> GetLikeByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<LikeResponseDto> CreateLikeAsync(LikeDto likeDto)
    {
        throw new NotImplementedException();
    }

    public Task<LikeResponseDto> UpdateLikeAsync(int id, LikeDto likeDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteLikeAsync(int id)
    {
        throw new NotImplementedException();
    }
}