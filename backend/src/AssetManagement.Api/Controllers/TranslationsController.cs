using AssetManagement.Core.Dtos;
using AssetManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    /// <summary>
    /// Internationalization &amp; Translation dictionary endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TranslationsController : ControllerBase
    {
        private readonly ITranslationService _translationService;

        public TranslationsController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        /// <summary>
        /// Retrieves public translation dictionary for unauthenticated users
        /// </summary>
        /// <param name="culture">Optional culture code (e.g. 'en')</param>
        /// <returns>Public translation dictionary key-values</returns>
        /// <response code="200">Public translation dictionary successfully retrieved</response>
        [HttpGet("public")]
        [ProducesResponseType(typeof(TranslationResponseDto), StatusCodes.Status200OK)]
        public IResult GetPublicTranslations([FromQuery] string? culture = "en")
        {
            TranslationResponseDto response = _translationService.GetPublicTranslations(culture);
            return TypedResults.Ok(response);
        }

        /// <summary>
        /// Retrieves platform translation dictionary for authenticated users
        /// </summary>
        /// <param name="culture">Optional culture code (e.g. 'en')</param>
        /// <returns>Authenticated translation dictionary key-values</returns>
        /// <response code="200">Authenticated translation dictionary successfully retrieved</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpGet("authenticated")]
        [ProducesResponseType(typeof(TranslationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IResult GetAuthenticatedTranslations([FromQuery] string? culture = "en")
        {
            TranslationResponseDto response = _translationService.GetAuthenticatedTranslations(culture);
            return TypedResults.Ok(response);
        }
    }
}
