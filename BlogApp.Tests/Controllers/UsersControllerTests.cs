using System.Security.Claims;
using BlogApp.API.Controllers;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogApp.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService = new();
    private readonly Mock<ILogger<UsersController>> _mockLogger = new();
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _controller = new UsersController(_mockAuthService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOk()
    {
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Username = null,
            Password = null
        };
        var response = new UserResponseDto { Id = Guid.NewGuid(), Email = request.Email };

        _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(response);

        var result = await _controller.Register(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(response, ok.Value);
    }

    [Fact]
    public async Task VerifyEmail_InvalidToken_ReturnsBadRequest()
    {
        _mockAuthService.Setup(s => s.VerifyEmailAsync("token", "user@example.com"))
            .ReturnsAsync(false);

        var result = await _controller.VerifyEmail("token", "user@example.com");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Invalid token", badRequest.Value);
    }

    // [Fact]
    // public async Task VerifyEmail_ValidToken_ReturnsOk()
    // {
    //     _mockAuthService.Setup(s => s.VerifyEmailAsync("token", "user@example.com"))
    //         .ReturnsAsync(true);
    //
    //     var result = await _controller.VerifyEmail("token", "user@example.com");
    //
    //     var ok = Assert.IsType<OkObjectResult>(result.Result);
    //
    //     // Use Json serialization to inspect value
    //     var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(ok.Value));
    //
    //     Assert.NotNull(dict);
    //     Assert.Equal("Email verified successfully", dict["message"]);
    // }

    [Fact]
    public async Task Login_ValidRequest_ReturnsOk()
    {
        var request = new LoginRequestDto { UsernameOrEmail = "user@example.com", Password = "pass" };
        var response = new UserResponseDto { Id = Guid.NewGuid(), Email = "user@example.com" };

        _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync(response);

        var result = await _controller.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(response, ok.Value);
    }

    [Fact]
    public async Task RefreshToken_ReturnsOk()
    {
        var request = new RefreshTokenRequestDto { AccessToken = "access", RefreshToken = "refresh" };
        var response = new UserResponseDto { Id = Guid.NewGuid(), Email = "user@example.com" };

        _mockAuthService.Setup(s => s.RefreshTokenAsync(request)).ReturnsAsync(response);

        var result = await _controller.RefreshToken(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(response, ok.Value);
    }

    [Fact]
    public async Task GetProfile_UserExists_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var profile = new ProfileResponseDto { Id = userId, Email = "user@example.com" };

        SetUserIdInContext(userId);

        _mockAuthService.Setup(s => s.GetProfileAsync(userId)).ReturnsAsync(profile);

        var result = await _controller.GetProfile();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(profile, ok.Value);
    }

    [Fact]
    public async Task GetProfile_UserNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        SetUserIdInContext(userId);

        _mockAuthService.Setup(s => s.GetProfileAsync(userId)).ReturnsAsync((ProfileResponseDto)null!);

        var result = await _controller.GetProfile();

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProfile_UserFound_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var dto = new UpdateProfileRequestDto { Bio = "Bio update test" };
        var updated = new ProfileResponseDto { Id = userId,  Bio = "Bio update test" , Email = "user@example.com" };

        SetUserIdInContext(userId);

        _mockAuthService.Setup(s => s.UpdateProfileAsync(userId, dto)).ReturnsAsync(updated);

        var result = await _controller.UpdateProfile(userId, dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(updated, ok.Value);
    }


    [Fact]
    public async Task UpdateProfile_UserNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var dto = new UpdateProfileRequestDto { Bio = "Bio update test" };

        SetUserIdInContext(userId);

        _mockAuthService.Setup(s => s.UpdateProfileAsync(userId, dto)).ReturnsAsync((Func<ProfileResponseDto>)null!);

        var result = await _controller.UpdateProfile(userId, dto);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    private void SetUserIdInContext(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}
