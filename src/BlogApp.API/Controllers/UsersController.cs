using System.Net;
using System.Security.Claims;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IAuthService authService, ILogger<UsersController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        logger.LogInformation("Registering new user: {email}", request.Email);
        var user = await authService.RegisterAsync(request);
        if (ModelState.IsValid == false)
        {
            logger.LogWarning("Invalid model state during registration: {modelState}", ModelState);
            return BadRequest(ModelState);
        }

        logger.LogInformation("User registered successfully: {userId}", user.Id);
        return Ok(user);
    }

    [HttpGet("verify-email")]
    public async Task<ActionResult<UserResponseDto>> VerifyEmail([FromQuery] string token, [FromQuery] string email)
    {
        email = WebUtility.UrlDecode(email);
        logger.LogInformation("Verifying email: {email} with token: {token}", email, token);
        var result = await authService.VerifyEmailAsync(token, email);
        if (result == false)
        {
            logger.LogWarning("Email verification failed for: {email}", email);
            return BadRequest("Invalid token");
        }

        logger.LogInformation("Email verified successfully: {email}", email);
        return Ok(new { message = "Email verified successfully" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        logger.LogInformation("Login attempt for: {usernameOrEmail}", request.UsernameOrEmail);
        var user = await authService.LoginAsync(request);
        if (ModelState.IsValid == false)
        {
            logger.LogWarning("Invalid model state during login for: {usernameOrEmail}", request.UsernameOrEmail);
            return BadRequest(ModelState);
        }

        logger.LogInformation("Login successful for userId: {userId}", user.Id);
        return Ok(user);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        logger.LogInformation("Refreshing token for accessToken: {accessToken}", request.AccessToken);
        var response = await authService.RefreshTokenAsync(request);
        logger.LogInformation("Token refreshed for userId: {userId}", response.Id);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileResponseDto>> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        logger.LogInformation("Fetching profile for userId: {userId}", userId);
        var user = await authService.GetProfileAsync(userId);
        if (user == null)
        {
            logger.LogWarning("Profile not found for userId: {userId}", userId);
            return NotFound();
        }

        logger.LogInformation("Profile retrieved for userId: {userId}", userId);
        return Ok(user);
    }

    [Authorize]
    [HttpPatch("update")]
    public async Task<ActionResult<UpdateProfileRequestDto>> UpdateProfile(Guid id, UpdateProfileRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        logger.LogInformation("Updating profile for userId: {userId} with target id: {id}", userId, id);
        var user = await authService.UpdateProfileAsync(id, request);
        if (user == null)
        {
            logger.LogWarning("Profile update failed. User not found with id: {id}", id);
            return NotFound();
        }

        if (ModelState.IsValid == false)
        {
            logger.LogWarning("Invalid model state during profile update for id: {id}", id);
            return BadRequest(ModelState);
        }

        logger.LogInformation("Profile updated successfully for id: {id}", id);
        return Ok(user);
    }
}