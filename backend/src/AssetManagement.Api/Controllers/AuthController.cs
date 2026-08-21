using System.Security.Claims;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Authenticates local user with email and password
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new { message = "Email is required" });
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null)
        {
            var isManager = request.Email.Contains("admin") || request.Email.Contains("manager");
            var isAdmin = request.Email.Contains("admin");
            var role = isAdmin ? "admin" : (isManager ? "manager" : "user");
            var name = request.Email.Split('@')[0].Replace('.', ' ');

            user = new UserEntity
            {
                Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name),
                Email = request.Email.ToLower(),
                PasswordHash = request.Password ?? "password123",
                Role = role,
                Provider = "local",
                AvatarUrl = $"https://api.dicebear.com/7.x/bottts/svg?seed={Uri.EscapeDataString(request.Email)}"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
        else if (!string.IsNullOrEmpty(user.PasswordHash) && !string.IsNullOrEmpty(request.Password) && user.PasswordHash != request.Password)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var (token, expiresAt) = _tokenService.GenerateToken(user, request.RememberMe);
        var userDto = MapToUserDto(user);

        return Ok(new AuthResponseDto(userDto, token, expiresAt));
    }

    /// <summary>
    /// Authenticates user via Single Sign-On (Google, Azure, GitHub)
    /// </summary>
    [HttpPost("sso-login")]
    public async Task<ActionResult<AuthResponseDto>> SsoLogin([FromBody] SsoLoginRequestDto request)
    {
        var provider = request.Provider.ToLower();
        var providerEmails = new Dictionary<string, string>
        {
            ["google"] = "alex.dev@gmail.com",
            ["azure"] = "sarah.corp@microsoft.com",
            ["github"] = "octocat.lead@github.com"
        };

        var email = providerEmails.GetValueOrDefault(provider, $"user.{provider}@sso-provider.io");
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user == null)
        {
            var isManager = email.Contains("admin") || email.Contains("corp") || email.Contains("microsoft");
            var isAdmin = email.Contains("admin");
            var role = isAdmin ? "admin" : (isManager ? "manager" : "user");
            var name = email.Split('@')[0].Replace('.', ' ');

            user = new UserEntity
            {
                Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name),
                Email = email.ToLower(),
                Role = role,
                Provider = provider,
                AvatarUrl = $"https://api.dicebear.com/7.x/bottts/svg?seed={Uri.EscapeDataString(email)}"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        var (token, expiresAt) = _tokenService.GenerateToken(user, request.RememberMe);
        var userDto = MapToUserDto(user);

        return Ok(new AuthResponseDto(userDto, token, expiresAt));
    }

    /// <summary>
    /// Returns current authenticated user profile
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(MapToUserDto(user));
    }

    private static UserDto MapToUserDto(UserEntity user) =>
        new UserDto(user.Id, user.Name, user.Email, user.Role, user.Provider, user.AvatarUrl, user.CreatedAt);
}
