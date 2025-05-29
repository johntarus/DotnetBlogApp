using System.Security.Claims;
using BlogApp.Data;
using BlogApp.Helpers;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using BlogApp.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(DatabaseContext context, IAuthService authService, IConfiguration config) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var user = await authService.RegisterAsync(request);
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var user = await authService.LoginAsync(request);
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            return Ok(user);
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileResponseDto>> GetProfile()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await authService.GetProfileAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [Authorize]
    [HttpPatch("update")]
    public async Task<ActionResult<UpdateProfileRequestDto>> UpdateProfile(UpdateProfileRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await context.Users.FindAsync(userId);
        if (user == null) return NotFound();
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        if(request.Bio != null)
            user.Bio = request.Bio;
        if(request.Avatar != null)
            user.Avatar = request.Avatar;
        user.UpdatedAt = DateTime.Now;
        await context.SaveChangesAsync();
        
        return Ok(new ProfileResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            Avatar = user.Avatar,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        });
    }
}