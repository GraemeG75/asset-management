using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Api.Controllers
{
    /// <summary>
    /// User profile management &amp; settings endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ProfileController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves the current authenticated user's profile
        /// </summary>
        /// <returns>User profile DTO</returns>
        /// <response code="200">User profile retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">User profile not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetProfile()
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

        /// <summary>
        /// Updates the authenticated user's entire profile (first name, last name, language, avatar)
        /// </summary>
        /// <param name="request">Full profile update payload</param>
        /// <returns>Updated user profile DTO</returns>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">Invalid profile payload</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">User profile not found</response>
        [HttpPut]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) && string.IsNullOrWhiteSpace(request.LastName))
            {
                return TypedResults.BadRequest(new { message = "First name or last name is required" });
            }

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

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            if (!string.IsNullOrWhiteSpace(request.PreferredLanguage))
            {
                user.PreferredLanguage = request.PreferredLanguage.Trim().ToLower();
            }
            if (request.AvatarUrl != null)
            {
                user.AvatarUrl = request.AvatarUrl;
            }

            await _dbContext.SaveChangesAsync();

            return TypedResults.Ok(MapToUserDto(user));
        }

        /// <summary>
        /// Updates user email address with format and database uniqueness validation
        /// </summary>
        /// <param name="request">New email payload</param>
        /// <returns>Updated user profile DTO</returns>
        /// <response code="200">Email updated successfully</response>
        /// <response code="400">Invalid email format or email already taken</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">User profile not found</response>
        [HttpPut("email")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> UpdateEmail([FromBody] UpdateEmailDto request)
        {
            if (string.IsNullOrWhiteSpace(request.NewEmail) || !new EmailAddressAttribute().IsValid(request.NewEmail))
            {
                return TypedResults.BadRequest(new { message = "A valid email address is required" });
            }

            string normalizedEmail = request.NewEmail.Trim().ToLower();

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

            bool emailTaken = await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != userId);
            if (emailTaken)
            {
                return TypedResults.BadRequest(new { message = "Email address is already in use by another account" });
            }

            user.Email = normalizedEmail;
            await _dbContext.SaveChangesAsync();

            return TypedResults.Ok(MapToUserDto(user));
        }

        /// <summary>
        /// Updates the authenticated user's preferred language in the database
        /// </summary>
        /// <param name="request">Language update payload</param>
        /// <returns>Updated user profile DTO</returns>
        /// <response code="200">Language updated successfully in database</response>
        /// <response code="400">Invalid language payload</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">User profile not found</response>
        [HttpPut("language")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> UpdateLanguage([FromBody] UpdateLanguageRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Language))
            {
                return TypedResults.BadRequest(new { message = "Language is required" });
            }

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

            user.PreferredLanguage = request.Language.ToLower();
            await _dbContext.SaveChangesAsync();

            return TypedResults.Ok(MapToUserDto(user));
        }

        private static UserDto MapToUserDto(UserEntity user) =>
            new UserDto(user.Id, user.FirstName, user.LastName, user.Name, user.Email, user.Role, user.Provider, user.AvatarUrl, user.PreferredLanguage, user.CreatedAt);
    }
}
