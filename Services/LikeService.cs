using AutoMapper;
using BlogApp.Dtos.Response;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Services;

public class LikeService(ILikeRepository likeRepo, IAuthRepository authRepository, IPostRepository postRepository, IMapper mapper) : ILikeService
{
    public async Task<PaginatedList<LikeResponseDto>> GetLikesAsync(int pageNumber, int pageSize)
    {
        var paginatedLikes = await likeRepo.GetLikesAsync(pageNumber, pageSize);
        var likes = mapper.Map<List<LikeResponseDto>>(paginatedLikes.Items);
        return new PaginatedList<LikeResponseDto>(likes, paginatedLikes.PageNumber, paginatedLikes.PageSize,
            paginatedLikes.TotalCount, paginatedLikes.TotalPages);
    }

    public async Task<LikeResponseDto?> GetLikeByIdAsync(int id)
    {
        var like = await likeRepo.GetLikeByIdAsync(id);
        return mapper.Map<LikeResponseDto>(like);
    }

    public async Task<LikeResponseDto?> CreateLikeAsync(LikeDto dto)
    {
        var existingLike = await likeRepo.GetLikeAsync(dto.PostId, dto.UserId);
        if (existingLike != null) return null;

        var user = await authRepository.GetUserByIdAsync(dto.UserId);
        if (user == null) throw new KeyNotFoundException("User not found");
        var post = await postRepository.GetPostByIdAsync(dto.PostId);
        if (post == null) throw new KeyNotFoundException("Post not found");
        
        var like = mapper.Map<Like>(dto);
        var created = await likeRepo.AddLikeAsync(like);

        return mapper.Map<LikeResponseDto>(created);
    }

    public async Task<bool> RemoveLikeAsync(LikeDto dto) =>
        await likeRepo.RemoveLikeAsync(dto.PostId, dto.UserId);

    public async Task<bool> CheckIfLikedAsync(CheckLikeRequestDto dto) =>
        await likeRepo.HasUserLikedPostAsync(dto.PostId, dto.UserId);
}