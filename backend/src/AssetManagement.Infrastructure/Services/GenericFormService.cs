using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Services.FormHandlers;

namespace AssetManagement.Infrastructure.Services
{
    public class GenericFormService : IGenericFormService
    {
        private readonly IMetadataRepository _metadataRepository;
        private readonly FormHandlerFactory _handlerFactory;
        private readonly ITranslationService _translationService;
        private readonly ISiteContext _siteContext;
        private readonly IUserContext _userContext;

        public GenericFormService(IMetadataRepository metadataRepository, FormHandlerFactory handlerFactory, ITranslationService translationService, ISiteContext siteContext, IUserContext userContext)
        {
            _metadataRepository = metadataRepository;
            _handlerFactory = handlerFactory;
            _translationService = translationService;
            _siteContext = siteContext;
            _userContext = userContext;
        }

        public async Task<FormSubmissionResultDto> SubmitFormAsync(FormSubmissionDto submission, string? userId = null, string? locale = null)
        {
            string effectiveLocale = !string.IsNullOrWhiteSpace(locale) ? locale : _siteContext.CurrentLocale;
            string effectiveUserId = !string.IsNullOrWhiteSpace(userId) ? userId : (_userContext.UserId ?? string.Empty);

            if (submission == null || string.IsNullOrWhiteSpace(submission.FormKey))
            {
                return new FormSubmissionResultDto
                {
                    Success = false,
                    Message = _translationService.GetString("ERR_INVALID_FORM_SUBMISSION", effectiveLocale) ?? "Invalid form submission request",
                    FormKey = submission?.FormKey ?? string.Empty
                };
            }

            // Look up form metadata from DB repository
            XFormEntity? formMeta = await _metadataRepository.GetFormByKeyAsync(submission.FormKey, effectiveLocale);
            
            IEnumerable<XMapperFlavorFieldEntity>? flavorFields = null;
            if (formMeta != null && !string.IsNullOrEmpty(formMeta.FlavorKey))
            {
                flavorFields = await _metadataRepository.GetFlavorFieldsAsync(formMeta.FlavorKey, effectiveLocale);
            }

            // Determine effective formType (from submission or metadata)
            string formType = !string.IsNullOrWhiteSpace(submission.FormType) 
                ? submission.FormType 
                : (formMeta?.FormType ?? "standard");

            FormSubmissionContext context = new FormSubmissionContext
            {
                Submission = submission,
                UserId = effectiveUserId,
                Locale = effectiveLocale,
                FormMetadata = formMeta,
                FlavorFields = flavorFields
            };

            // Resolve specialized form handler based on formType
            IFormTypeHandler handler = _handlerFactory.GetHandler(formType);
            return await handler.HandleSubmissionAsync(context);
        }
    }
}
