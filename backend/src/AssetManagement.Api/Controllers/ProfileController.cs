using System.ComponentModel.DataAnnotations;
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
    /// User profile management &amp; settings endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ITranslationService _translationService;

        public ProfileController(AppDbContext dbContext, ITranslationService translationService)
        {
            _dbContext = dbContext;
            _translationService = translationService;
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
                string msg = _translationService.GetString("ERR_USER_NOT_FOUND", GetUserLocale(null));
                return TypedResults.NotFound(new { message = msg });
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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return TypedResults.Unauthorized();
            }

            UserEntity? user = await _dbContext.Users.FindAsync(userId);
            string locale = GetUserLocale(user);

            if (user == null)
            {
                string msg = _translationService.GetString("ERR_USER_NOT_FOUND", locale);
                return TypedResults.NotFound(new { message = msg });
            }

            if (string.IsNullOrWhiteSpace(request.FirstName) && string.IsNullOrWhiteSpace(request.LastName))
            {
                string msg = _translationService.GetString("ERR_FIRST_LAST_NAME_REQUIRED", locale);
                return TypedResults.BadRequest(new { message = msg });
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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return TypedResults.Unauthorized();
            }

            UserEntity? user = await _dbContext.Users.FindAsync(userId);
            string locale = GetUserLocale(user);

            if (user == null)
            {
                string msg = _translationService.GetString("ERR_USER_NOT_FOUND", locale);
                return TypedResults.NotFound(new { message = msg });
            }

            if (string.IsNullOrWhiteSpace(request.NewEmail) || !new EmailAddressAttribute().IsValid(request.NewEmail))
            {
                string msg = _translationService.GetString("ERR_EMAIL_INVALID", locale);
                return TypedResults.BadRequest(new { message = msg });
            }

            string normalizedEmail = request.NewEmail.Trim().ToLower();

            bool emailTaken = await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != userId);
            if (emailTaken)
            {
                string msg = _translationService.GetString("ERR_EMAIL_TAKEN", locale);
                return TypedResults.BadRequest(new { message = msg });
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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return TypedResults.Unauthorized();
            }

            UserEntity? user = await _dbContext.Users.FindAsync(userId);
            string locale = GetUserLocale(user);

            if (user == null)
            {
                string msg = _translationService.GetString("ERR_USER_NOT_FOUND", locale);
                return TypedResults.NotFound(new { message = msg });
            }

            if (string.IsNullOrWhiteSpace(request.Language))
            {
                string msg = _translationService.GetString("ERR_LANGUAGE_REQUIRED", locale);
                return TypedResults.BadRequest(new { message = msg });
            }

            user.PreferredLanguage = request.Language.ToLower();
            await _dbContext.SaveChangesAsync();

            return TypedResults.Ok(MapToUserDto(user));
        }

        private string GetUserLocale(UserEntity? user)
        {
            if (user != null && !string.IsNullOrWhiteSpace(user.PreferredLanguage))
            {
                return user.PreferredLanguage;
            }

            string? requestLocale = Request?.Query["locale"].ToString();
            if (!string.IsNullOrWhiteSpace(requestLocale))
            {
                return requestLocale;
            }

            return "en-US";
        }

        private static UserDto MapToUserDto(UserEntity user) =>
            new UserDto(user.Id, user.FirstName, user.LastName, user.Name, user.Email, user.Role, user.Provider, user.AvatarUrl, user.PreferredLanguage, user.CreatedAt);
    }
}
