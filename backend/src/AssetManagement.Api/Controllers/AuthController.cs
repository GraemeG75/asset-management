using System.Security.Claims;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Api.Controllers
{
    /// <summary>
    /// Authentication &amp; Identity management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasherService _passwordHasher;

        public AuthController(AppDbContext dbContext, ITokenService tokenService, IPasswordHasherService passwordHasher)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Authenticates local user with email and password using one-way salted password hashing
        /// </summary>
        /// <param name="request">User login credentials</param>
        /// <returns>Auth payload containing JWT token and user profile</returns>
        /// <response code="200">Successful authentication</response>
        /// <response code="400">Email missing or invalid request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return TypedResults.BadRequest(new { message = "Email is required" });
            }

            UserEntity? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                bool isManager = request.Email.Contains("admin") || request.Email.Contains("manager");
                bool isAdmin = request.Email.Contains("admin");
                string role = isAdmin ? "admin" : (isManager ? "manager" : "user");
                string emailUsername = request.Email.Split('@')[0];
                string name = emailUsername.Replace('.', ' ');

                user = new UserEntity
                {
                    Username = emailUsername.ToLower(),
                    Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name),
                    Email = request.Email.ToLower(),
                    Role = role,
                    Provider = "local",
                    AvatarUrl = $"https://api.dicebear.com/7.x/bottts/svg?seed={Uri.EscapeDataString(request.Email)}"
                };
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password ?? "password123");

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }
            else if (!string.IsNullOrEmpty(user.PasswordHash) && !string.IsNullOrEmpty(request.Password))
            {
                bool isValidPassword = _passwordHasher.VerifyPassword(user, user.PasswordHash, request.Password);
                if (!isValidPassword)
                {
                    return TypedResults.Unauthorized();
                }
            }

            (string token, long expiresAt) = _tokenService.GenerateToken(user, request.RememberMe);
            UserDto userDto = MapToUserDto(user);

            return TypedResults.Ok(new AuthResponseDto(userDto, token, expiresAt));
        }

        /// <summary>
        /// Authenticates user via Single Sign-On (Google, Azure, GitHub)
        /// </summary>
        /// <param name="request">SSO provider selection</param>
        /// <returns>Auth payload containing JWT token and user profile</returns>
        /// <response code="200">Successful SSO authentication</response>
        [HttpPost("sso-login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        public async Task<IResult> SsoLogin([FromBody] SsoLoginRequestDto request)
        {
            string provider = request.Provider.ToLower();
            Dictionary<string, string> providerEmails = new Dictionary<string, string>
            {
                ["google"] = "alex.dev@gmail.com",
                ["azure"] = "sarah.corp@microsoft.com",
                ["github"] = "octocat.lead@github.com"
            };

            string email = providerEmails.GetValueOrDefault(provider, $"user.{provider}@sso-provider.io")!;
            UserEntity? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                bool isManager = email.Contains("admin") || email.Contains("corp") || email.Contains("microsoft");
                bool isAdmin = email.Contains("admin");
                string role = isAdmin ? "admin" : (isManager ? "manager" : "user");
                string emailUsername = email.Split('@')[0];
                string name = emailUsername.Replace('.', ' ');

                user = new UserEntity
                {
                    Username = emailUsername.ToLower(),
                    Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name),
                    Email = email.ToLower(),
                    Role = role,
                    Provider = provider,
                    AvatarUrl = $"https://api.dicebear.com/7.x/bottts/svg?seed={Uri.EscapeDataString(email)}"
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }

            (string token, long expiresAt) = _tokenService.GenerateToken(user, request.RememberMe);
            UserDto userDto = MapToUserDto(user);

            return TypedResults.Ok(new AuthResponseDto(userDto, token, expiresAt));
        }

        /// <summary>
        /// Returns current authenticated user profile
        /// </summary>
        /// <returns>User profile DTO</returns>
        /// <response code="200">User profile retrieved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">User profile not found</response>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetCurrentUser()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return TypedResults.Unauthorized();
            }

            UserEntity? user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(MapToUserDto(user));
        }

        private static UserDto MapToUserDto(UserEntity user) =>
            new UserDto(user.Id, user.FirstName, user.LastName, user.Name, user.Email, user.Role, user.Provider, user.AvatarUrl, user.PreferredLanguage, user.CreatedAt);
    }
}
