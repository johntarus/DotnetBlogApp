using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using BlogApp.Security;

namespace BlogApp.Services;

public class AuthService(IAuthRepository authRepository, IConfiguration config) : IAuthService
{
    public async Task<UserResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if(await authRepository.ExistsByEmailAsync(request.Email))
            throw new ApplicationException("Email already exists");
        var registerUser = new User()
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordHelper.HashPassword(request.Password)
        };

        var user = await authRepository.CreateAsync(registerUser);
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Token = JwtHelper.GenerateToken(user, config)
        };
    }

    public Task<UserResponseDto> LoginAsync(LoginRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task<ProfileResponseDto> GetProfileAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<UpdateProfileRequestDto> UpdatePrifileAsync(Guid id, UpdateProfileRequestDto request)
    {
        throw new NotImplementedException();
    }
}