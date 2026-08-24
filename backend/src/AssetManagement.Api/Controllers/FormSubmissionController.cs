using System;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/form-data")]
    [Produces("application/json")]
    public class FormSubmissionController : ControllerBase
    {
        private readonly IGenericFormService _genericFormService;
        private readonly IUserContext _userContext;
        private readonly ISiteContext _siteContext;

        public FormSubmissionController(IGenericFormService genericFormService, IUserContext userContext, ISiteContext siteContext)
        {
            _genericFormService = genericFormService;
            _userContext = userContext;
            _siteContext = siteContext;
        }

        /// <summary>
        /// Generic Form Submission endpoint handling dynamic metadata forms across all form types (standard, detail, grid, search, widget)
        /// </summary>
        /// <param name="submission">Dynamic form submission payload</param>
        /// <returns>Form submission outcome result</returns>
        [HttpPost("submit")]
        [ProducesResponseType(typeof(FormSubmissionResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IResult> SubmitForm([FromBody] FormSubmissionDto submission)
        {
            if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
            {
                throw new UnauthorizedAccessException("User identity claim is missing or invalid.");
            }

            string locale = _siteContext.CurrentLocale;
            FormSubmissionResultDto result = await _genericFormService.SubmitFormAsync(submission, _userContext.UserId, locale);
            if (!result.Success)
            {
                return TypedResults.BadRequest(result);
            }

            return TypedResults.Ok(result);
        }

        /// <summary>
        /// Form submission helper route specifying formKey in path
        /// </summary>
        [HttpPost("{formKey}")]
        [ProducesResponseType(typeof(FormSubmissionResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IResult> SubmitFormWithKey(string formKey, [FromBody] FormSubmissionDto submission)
        {
            submission.FormKey = formKey;
            return await SubmitForm(submission);
        }
    }
}
