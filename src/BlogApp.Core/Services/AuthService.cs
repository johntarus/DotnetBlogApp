using System.Security.Claims;
using AutoMapper;
using BlogApp.Core.Common.Helpers;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Core.Utils;
using BlogApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BlogApp.Core.Services;

public class AuthService(
    IAuthRepository authRepository,
    IConfiguration config,
    IEmailService emailService,
    ILogger<AuthService> logger,
    IMapper mapper
) : IAuthService
{
    public async Task<UserResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        logger.LogInformation("Registering new user: {email}", request.Email);
        if(await authRepository.ExistsByEmailAsync(request.Email))
        {
            logger.LogWarning("Email already exists: {email}", request.Email);
            throw new ApplicationException("Email already exists");
        }

        var registerUser = mapper.Map<User>(request);
        var user = await authRepository.CreateAsync(registerUser);
        logger.LogInformation("User created with ID: {userId}", user.Id);

        await SendVerificationEmailAsync(user);

        return mapper.Map<UserResponseDto>(user);
    }

    private async Task SendVerificationEmailAsync(User user)
    {
        if (string.IsNullOrEmpty(user.Email))
        {
            logger.LogWarning("User email is empty, cannot send verification email.");
            return;
        }

        try
        {
            var verificationLink = EmailVerificationUtils.GenerateVerificationLink(
                config["Client:BaseUrl"], user.EmailVerificationToken, user.Email);

            var emailBody = EmailTemplateUtils.GetVerificationEmailTemplate(user.Username, verificationLink);

            await emailService.SendEmailAsync(user.Email, "Verify your email address", emailBody);
            logger.LogInformation("Verification email sent to {email}", user.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send verification email to {email}", user.Email);
            throw new ApplicationException("Failed to send verification email", ex);
        }
    }

    public async Task<bool> VerifyEmailAsync(string token, string email)
    {
        logger.LogInformation("Verifying email: {email} with token: {token}", email, token);
        var user = await authRepository.GetByEmailAsync(email);
        if (user == null)
        {
            logger.LogWarning("User not found with email: {email}", email);
            throw new ApplicationException("User not found");
        }

        if (user.IsEmailVerified)
        {
            logger.LogWarning("Email already verified for user: {email}", email);
            throw new ApplicationException("Email already verified");
        }

        if (user.EmailVerificationToken != token || user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning("Invalid or expired token for email: {email}", email);
            throw new ApplicationException("Invalid or expired verification token");
        }

        user.IsActive = true;
        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiresAt = null;
        await authRepository.UpdateAsync(user);

        logger.LogInformation("Email verified for user: {email}", email);
        return true;
    }

    public async Task<UserResponseDto> LoginAsync(LoginRequestDto request)
    {
        logger.LogInformation("User attempting login: {usernameOrEmail}", request.UsernameOrEmail);
        var user = await authRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail);

        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
        {
            logger.LogWarning("Missing credentials");
            throw new ArgumentException("Username or Email and Password are required");
        }

        if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Invalid login attempt for {usernameOrEmail}", request.UsernameOrEmail);
            throw new UnauthorizedAccessException("Invalid Credentials");
        }

        if (!user.IsEmailVerified)
        {
            logger.LogWarning("Login attempt with unverified email: {email}", user.Email);
            throw new UnauthorizedAccessException("Email not verified. Please check your inbox.");
        }

        var accessToken = JwtHelper.GenerateAccessToken(user, config);
        var refreshToken = JwtHelper.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await authRepository.UpdateAsync(user);

        var loginUser = mapper.Map<UserResponseDto>(user);
        loginUser.AccessToken = accessToken;
        loginUser.RefreshToken = refreshToken;

        logger.LogInformation("Login successful for user: {email}", user.Email);
        return loginUser;
    }

    public async Task<UserResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        logger.LogInformation("Refreshing token for access token: {accessToken}", request.AccessToken);

        var principle = JwtHelper.GetPrincipleFromExpiredToken(request.AccessToken, config);
        var userId = Guid.Parse(principle.FindFirst(ClaimTypes.NameIdentifier).Value);

        var user = await authRepository.GetUserByIdAsync(userId);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning("Invalid refresh token for userId: {userId}", userId);
            throw new SecurityTokenException("Invalid Refresh Token");
        }

        var loginResponse = mapper.Map<UserResponseDto>(user);
        loginResponse.AccessToken = JwtHelper.GenerateAccessToken(user, config);
        loginResponse.RefreshToken = JwtHelper.GenerateRefreshToken();

        user.RefreshToken = loginResponse.RefreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await authRepository.UpdateAsync(user);

        logger.LogInformation("Refresh token issued for userId: {userId}", userId);
        return loginResponse;
    }

    public async Task<ProfileResponseDto> GetProfileAsync(Guid userId)
    {
        logger.LogInformation("Fetching profile for userId: {userId}", userId);
        var user = await authRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("Profile not found for userId: {userId}", userId);
            throw new ApplicationException("User not found");
        }

        return mapper.Map<ProfileResponseDto>(user);
    }

    public async Task<ProfileResponseDto> UpdateProfileAsync(Guid id, UpdateProfileRequestDto request)
    {
        logger.LogInformation("Updating profile for userId: {userId}", id);
        var user = await authRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            logger.LogWarning("User not found with id: {userId}", id);
            return null;
        }

        mapper.Map(request, user);
        var updatedUser = await authRepository.UpdateAsync(user);
        logger.LogInformation("Profile updated for userId: {userId}", id);
        return mapper.Map<ProfileResponseDto>(updatedUser);
    }
}