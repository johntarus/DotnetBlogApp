using System.Security.Claims;
using BlogApp.Data;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<UpdateProfileRequestDto>> UpdateProfile(Guid id, UpdateProfileRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await authService.UpdatePrifileAsync(id, request);
        if (user == null) return NotFound();
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        return Ok(user);
    }
}