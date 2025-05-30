using System.Security.Claims;
using BlogApp.Data;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IAuthService authService, IConfiguration config) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
            var user = await authService.RegisterAsync(request);
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginRequestDto request)
    {
            var user = await authService.LoginAsync(request);
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            return Ok(user);
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
        var user = await authService.UpdatePrifileAsync(id, request);
        if (user == null) return NotFound();
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        return Ok(user);
    }
}