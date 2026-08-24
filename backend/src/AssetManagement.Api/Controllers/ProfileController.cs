using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
        private readonly IUserContext _userContext;
        private readonly ISiteContext _siteContext;

        public ProfileController(AppDbContext dbContext, ITranslationService translationService, IUserContext userContext, ISiteContext siteContext)
        {
            _dbContext = dbContext;
            _translationService = translationService;
            _userContext = userContext;
            _siteContext = siteContext;
        }

        /// <summary>
        /// Retrieves the current authenticated user's profile
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetProfile()
        {
            UserEntity? user = await _userContext.GetCurrentUserEntityAsync();
            if (user == null)
            {
                string msg = _translationService.GetString("ERR_USER_NOT_FOUND", _siteContext.CurrentLocale);
                return TypedResults.NotFound(new { message = msg });
            }

            return TypedResults.Ok(MapToUserDto(user));
        }

        /// <summary>
        /// Updates the authenticated user's entire profile (first name, last name, language, avatar)
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> UpdateProfile([FromBody] Dictionary<string, object?> request)
        {
            UserEntity? user = await _userContext.GetCurrentUserEntityAsync();
            string locale = _siteContext.CurrentLocale;

            if (user == null)
            {
                string msg = _translationService.GetString("ERR_USER_NOT_FOUND", locale);
                return TypedResults.NotFound(new { message = msg });
            }

            if (request.TryGetValue("firstName", out object? fnObj) && fnObj != null)
            {
                user.FirstName = fnObj.ToString()!.Trim();
            }

            if (request.TryGetValue("lastName", out object? lnObj) && lnObj != null)
            {
                user.LastName = lnObj.ToString()!.Trim();
            }

            if (request.TryGetValue("preferredLanguage", out object? langObj) && langObj != null)
            {
                user.PreferredLanguage = langObj.ToString()!.Trim().ToLower();
            }

            if (request.TryGetValue("avatarUrl", out object? avObj) && avObj != null)
            {
                user.AvatarUrl = avObj.ToString();
            }

            await _dbContext.SaveChangesAsync();

            return TypedResults.Ok(MapToUserDto(user));
        }

        /// <summary>
        /// Updates user email address with format and database uniqueness validation
        /// </summary>
        [HttpPut("email")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> UpdateEmail([FromBody] Dictionary<string, string> request)
        {
            UserEntity? user = await _userContext.GetCurrentUserEntityAsync();
            string locale = _siteContext.CurrentLocale;

            if (user == null)
            {
                string msg = _translationService.GetString("ERR_USER_NOT_FOUND", locale);
                return TypedResults.NotFound(new { message = msg });
            }

            if (!request.TryGetValue("newEmail", out string? newEmail) || string.IsNullOrWhiteSpace(newEmail) || !new EmailAddressAttribute().IsValid(newEmail))
            {
                string msg = _translationService.GetString("ERR_EMAIL_INVALID", locale);
                return TypedResults.BadRequest(new { message = msg });
            }

            string normalizedEmail = newEmail.Trim().ToLower();

            bool emailTaken = await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != user.Id);
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
        [HttpPut("language")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> UpdateLanguage([FromBody] Dictionary<string, string> request)
        {
            UserEntity? user = await _userContext.GetCurrentUserEntityAsync();
            string locale = _siteContext.CurrentLocale;

            if (user == null)
            {
                string msg = _translationService.GetString("ERR_USER_NOT_FOUND", locale);
                return TypedResults.NotFound(new { message = msg });
            }

            if (!request.TryGetValue("language", out string? language) || string.IsNullOrWhiteSpace(language))
            {
                string msg = _translationService.GetString("ERR_LANGUAGE_REQUIRED", locale);
                return TypedResults.BadRequest(new { message = msg });
            }

            user.PreferredLanguage = language.ToLower();
            await _dbContext.SaveChangesAsync();

            return TypedResults.Ok(MapToUserDto(user));
        }

        private static UserDto MapToUserDto(UserEntity user)
        {
            return new UserDto(user.Id, user.FirstName, user.LastName, user.Name, user.Email, user.Role, user.Provider, user.AvatarUrl, user.PreferredLanguage, user.CreatedAt);
        }
    }
}
