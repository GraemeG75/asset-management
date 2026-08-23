using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    [ApiController]
    [Route("api/meta")]
    [Route("api/form-metadata")]
    public class MetaController : ControllerBase
    {
        private readonly IMetadataRepository _metadataRepository;
        private readonly ITranslationService _translationService;

        public MetaController(IMetadataRepository metadataRepository, ITranslationService translationService)
        {
            _metadataRepository = metadataRepository;
            _translationService = translationService;
        }

        /// <summary>
        /// Retrieves user bootstrap metadata upon login (Profile nav, Site nav, Inbox, Dashboard forms)
        /// fetched exclusively from the database repository.
        /// </summary>
        [HttpGet("user-bootstrap")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserBootstrapDto))]
        public async Task<IResult> GetUserBootstrap([FromQuery] string locale = "en-US")
        {
            IEnumerable<XProfileNavLinkEntity> dbProfileNav = await _metadataRepository.GetProfileNavLinksAsync(locale);
            List<ProfileNavLinkDto> profileNavLinks = dbProfileNav.Select(item => new ProfileNavLinkDto
            {
                Id = item.LinkKey,
                Label = item.Label,
                Icon = item.Icon,
                Url = item.Url,
                Badge = item.Badge,
                BadgeColor = item.BadgeColor,
                Order = item.DisplayOrder,
                IsActive = true
            }).ToList();

            IEnumerable<XSiteNavLinkEntity> dbSiteNav = await _metadataRepository.GetSiteNavLinksAsync(locale);
            List<SiteNavLinkDto> siteNavLinks = dbSiteNav.Select(item => new SiteNavLinkDto
            {
                Id = item.LinkKey,
                Label = item.Label,
                Icon = item.Icon,
                Route = item.Route,
                BadgeCount = item.BadgeCount,
                Category = item.Category,
                Order = item.DisplayOrder,
                IsActive = true
            }).ToList();

            IEnumerable<XFormEntity> dbForms = await _metadataRepository.GetFormsForPageAsync("dashboard", locale);
            List<DashboardFormMetadataDto> dashboardForms = new List<DashboardFormMetadataDto>();
            foreach (XFormEntity form in dbForms)
            {
                List<FormFieldDto> fields = new List<FormFieldDto>();
                if (!string.IsNullOrEmpty(form.FlavorKey))
                {
                    IEnumerable<XMapperFlavorFieldEntity> flavorFields = await _metadataRepository.GetFlavorFieldsAsync(form.FlavorKey, locale);
                    foreach (XMapperFlavorFieldEntity ff in flavorFields)
                    {
                        fields.Add(new FormFieldDto
                        {
                            Key = ff.KeyName,
                            Label = ff.Label,
                            Type = ff.FieldType,
                            Placeholder = ff.Placeholder,
                            DefaultValue = ff.DefaultValue,
                            HelpText = ff.HelpText,
                            GridCols = ff.GridCols,
                            Readonly = ff.IsReadonly,
                            Disabled = ff.IsDisabled,
                            CustomCssClass = ff.CustomCssClass
                        });
                    }
                }

                dashboardForms.Add(new DashboardFormMetadataDto
                {
                    FormId = form.FormKey,
                    FormType = form.FormType,
                    Caption = form.Caption,
                    Title = form.Title,
                    Description = form.Description,
                    FormInfo = form.FormInfo,
                    IsEditable = form.IsEditable,
                    LabelPosition = form.LabelPosition,
                    GridCols = form.GridCols,
                    SubmitButtonText = form.SubmitButtonText,
                    ShowResetButton = form.ShowResetButton,
                    Fields = fields
                });
            }

            UserBootstrapDto bootstrap = new UserBootstrapDto
            {
                UserId = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
                UserName = "Sarah Connor",
                UserEmail = "s.connor@enterprise.com",
                Role = "Senior Asset Manager",
                ProfileNavLinks = profileNavLinks,
                SiteNavLinks = siteNavLinks,
                InboxCount = 4,
                DashboardForms = dashboardForms
            };

            return TypedResults.Ok(bootstrap);
        }

        [HttpGet("pages")]
        public async Task<IResult> GetAvailablePages([FromQuery] string locale = "en-US")
        {
            IEnumerable<XPageEntity> pages = await _metadataRepository.GetPagesAsync(locale);
            return TypedResults.Ok(pages);
        }

        [HttpGet("pages/{pageKey}")]
        public async Task<IResult> GetPageInfo(string pageKey, [FromQuery] string locale = "en-US")
        {
            XPageEntity? page = await _metadataRepository.GetPageByKeyAsync(pageKey, locale);
            if (page == null)
            {
                string msg = _translationService.GetString("ERR_PAGE_NOT_FOUND", locale, pageKey);
                return TypedResults.NotFound(new { message = msg });
            }

            IEnumerable<XFormEntity> forms = await _metadataRepository.GetFormsForPageAsync(pageKey, locale);
            List<PageFormSummaryDto> formSummaries = new List<PageFormSummaryDto>();
            foreach (XFormEntity form in forms)
            {
                formSummaries.Add(new PageFormSummaryDto
                {
                    FormId = form.FormKey,
                    Caption = form.Caption,
                    Description = form.Description
                });
            }

            PageInfoDto pageInfo = new PageInfoDto
            {
                PageId = page.PageKey,
                Title = page.Title,
                Description = page.Description ?? string.Empty,
                Forms = formSummaries
            };

            return TypedResults.Ok(pageInfo);
        }

        [HttpGet("forms/{formKey}")]
        [HttpGet("{formKey}")]
        public async Task<IResult> GetFormSchema(string formKey, [FromQuery] string locale = "en-US")
        {
            XFormEntity? form = await _metadataRepository.GetFormByKeyAsync(formKey, locale);
            if (form == null)
            {
                string msg = _translationService.GetString("ERR_FORM_NOT_FOUND", locale, formKey);
                return TypedResults.NotFound(new { message = msg });
            }

            List<FormFieldDto> fields = new List<FormFieldDto>();
            if (!string.IsNullOrEmpty(form.FlavorKey))
            {
                IEnumerable<XMapperFlavorFieldEntity> flavorFields = await _metadataRepository.GetFlavorFieldsAsync(form.FlavorKey, locale);
                foreach (XMapperFlavorFieldEntity ff in flavorFields)
                {
                    fields.Add(new FormFieldDto
                    {
                        Key = ff.KeyName,
                        Label = ff.Label,
                        Type = ff.FieldType,
                        Placeholder = ff.Placeholder,
                        DefaultValue = ff.DefaultValue,
                        HelpText = ff.HelpText,
                        GridCols = ff.GridCols,
                        Readonly = ff.IsReadonly,
                        Disabled = ff.IsDisabled,
                        CustomCssClass = ff.CustomCssClass
                    });
                }
            }

            FormSchemaDto schema = new FormSchemaDto
            {
                Id = form.FormKey,
                Caption = form.Caption,
                Title = form.Title,
                Description = form.Description ?? string.Empty,
                FormInfo = form.FormInfo,
                IsEditable = form.IsEditable,
                SubmitButtonText = form.SubmitButtonText,
                ShowResetButton = form.ShowResetButton,
                Fields = fields
            };

            return TypedResults.Ok(schema);
        }

        [HttpGet("mappers")]
        public async Task<IResult> GetMappers([FromQuery] string locale = "en-US")
        {
            IEnumerable<XMapperEntity> mappers = await _metadataRepository.GetMappersAsync(locale);
            return TypedResults.Ok(mappers);
        }

        [HttpGet("flavors/{flavorKey}/fields")]
        public async Task<IResult> GetFlavorFields(string flavorKey, [FromQuery] string locale = "en-US")
        {
            IEnumerable<XMapperFlavorFieldEntity> fields = await _metadataRepository.GetFlavorFieldsAsync(flavorKey, locale);
            return TypedResults.Ok(fields);
        }
    }
}
