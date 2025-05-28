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

    public async Task<LikeResponseDto> GetLikeByIdAsync(int id)
    {
        var like = await likeRepository.GetLikeByIdAsync(id);
        return new LikeResponseDto
        {
            Id = like.Id,
            UserId = like.UserId,
            PostId = like.PostId,
            Username = like.User?.Username,
            CreatedAt = like.CreateAt
        };
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