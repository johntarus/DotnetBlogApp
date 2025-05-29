using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface IAuthService
{
    Task<UserResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<UserResponseDto> LoginAsync(LoginRequestDto request);
    Task<ProfileResponseDto> GetProfileAsync(Guid userId);
    Task<ProfileResponseDto> UpdatePrifileAsync(Guid id, UpdateProfileRequestDto request);
}