using System.Net;
using System.Security.Claims;
using BlogApp.Dtos.Request;
using BlogApp.Dtos.Response;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
            var user = await authService.RegisterAsync(request);
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            return Ok(user);
    }

    [HttpGet("verify-email")]
    public async Task<ActionResult<UserResponseDto>> VerifyEmail([FromQuery] string token, [FromQuery] string email)
    {
        email = WebUtility.UrlDecode(email);
        var result = await authService.VerifyEmailAsync(token, email);
        if (result == false) return BadRequest("Invalid token");
        return Ok(new { message = "Email verified successfully" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginRequestDto request)
    {
            var user = await authService.LoginAsync(request);
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            return Ok(user);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var response = await authService.RefreshTokenAsync(request);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileResponseDto>> GetProfile()
    {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await authService.GetProfileAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
    }

    [Authorize]
    [HttpPatch("update")]
    public async Task<ActionResult<UpdateProfileRequestDto>> UpdateProfile(Guid id, UpdateProfileRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await authService.UpdateProfileAsync(id, request);
        if (user == null) return NotFound();
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        return Ok(user);
    }
}