using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Services;

public class LikeService(ILikeRepository likeRepo) : ILikeService
{
    public async Task<List<LikeResponseDto>> GetLikesAsync()
    {
        var likes = await likeRepo.GetLikesAsync();
        return likes.Select(l => new LikeResponseDto
        {
            Id = l.Id,
            UserId = l.UserId,
            PostId = l.PostId,
            Username = l.User?.Username,
            CreatedAt = l.CreateAt
        }).ToList();
    }

    public async Task<LikeResponseDto?> GetLikeByIdAsync(int id)
    {
        var like = await likeRepo.GetLikeByIdAsync(id);
        return like == null ? null : new LikeResponseDto
        {
            Id = like.Id,
            PostId = like.PostId,
            UserId = like.UserId,
            Username = like.User?.Username,
            CreatedAt = like.CreateAt
        };
    }

    public async Task<LikeResponseDto?> CreateLikeAsync(LikeDto dto)
    {
        var existingLike = await likeRepo.GetLikeAsync(dto.PostId, dto.UserId);
        if (existingLike != null) return null;

        var like = new Like { PostId = dto.PostId, UserId = dto.UserId };
        var created = await likeRepo.AddLikeAsync(like);

        return new LikeResponseDto
        {
            Id = created.Id,
            PostId = created.PostId,
            UserId = created.UserId,
            Username = created.User?.Username,
            CreatedAt = created.CreateAt
        };
    }

    public async Task<bool> RemoveLikeAsync(LikeDto dto) =>
        await likeRepo.RemoveLikeAsync(dto.PostId, dto.UserId);

    public async Task<bool> CheckIfLikedAsync(CheckLikeRequestDto dto) =>
        await likeRepo.HasUserLikedPostAsync(dto.PostId, dto.UserId);
}