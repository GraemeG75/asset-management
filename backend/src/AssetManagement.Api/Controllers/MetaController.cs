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

        public MetaController(IMetadataRepository metadataRepository)
        {
            _metadataRepository = metadataRepository;
        }

        /// <summary>
        /// Retrieves user bootstrap metadata upon login (Profile nav, Site nav, Inbox, Dashboard form types)
        /// </summary>
        [HttpGet("user-bootstrap")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserBootstrapDto))]
        public async Task<IResult> GetUserBootstrap([FromQuery] string locale = "en-US")
        {
            List<ProfileNavLinkDto> profileNavLinks = new List<ProfileNavLinkDto>();
            try
            {
                IEnumerable<XProfileNavLinkEntity> dbProfileNav = await _metadataRepository.GetProfileNavLinksAsync(locale);
                foreach (XProfileNavLinkEntity item in dbProfileNav)
                {
                    profileNavLinks.Add(new ProfileNavLinkDto
                    {
                        Id = item.LinkKey,
                        Label = item.Label,
                        Icon = item.Icon,
                        Url = item.Url,
                        Badge = item.Badge,
                        BadgeColor = item.BadgeColor,
                        Order = item.DisplayOrder,
                        IsActive = true
                    });
                }
            }
            catch
            {
                // Fallback to default nav links if repository query fails
            }

            List<SiteNavLinkDto> siteNavLinks = new List<SiteNavLinkDto>();
            try
            {
                IEnumerable<XSiteNavLinkEntity> dbSiteNav = await _metadataRepository.GetSiteNavLinksAsync(locale);
                foreach (XSiteNavLinkEntity item in dbSiteNav)
                {
                    siteNavLinks.Add(new SiteNavLinkDto
                    {
                        Id = item.LinkKey,
                        Label = item.Label,
                        Icon = item.Icon,
                        Route = item.Route,
                        BadgeCount = item.BadgeCount,
                        Category = item.Category,
                        Order = item.DisplayOrder,
                        IsActive = true
                    });
                }
            }
            catch
            {
                // Fallback to default nav links if repository query fails
            }

            // Fallback seed defaults if DB views return empty
            if (profileNavLinks.Count == 0)
            {
                profileNavLinks.Add(new ProfileNavLinkDto { Id = "profile-settings", Label = "My Profile", Icon = "user", Url = "/profile", Order = 1, IsActive = true });
                profileNavLinks.Add(new ProfileNavLinkDto { Id = "profile-security", Label = "Security & Credentials", Icon = "shield", Url = "/profile/security", Order = 2, IsActive = true });
                profileNavLinks.Add(new ProfileNavLinkDto { Id = "profile-preferences", Label = "Language & Preferences", Icon = "sliders", Url = "/profile/preferences", Order = 3, IsActive = true });
                profileNavLinks.Add(new ProfileNavLinkDto { Id = "profile-help", Label = "Help & Documentation", Icon = "help-circle", Url = "/help", Order = 4, IsActive = true });
                profileNavLinks.Add(new ProfileNavLinkDto { Id = "profile-logout", Label = "Log Out", Icon = "log-out", Url = "/logout", Order = 5, IsActive = true });
            }

            if (siteNavLinks.Count == 0)
            {
                siteNavLinks.Add(new SiteNavLinkDto { Id = "nav-dashboard", Label = "Inbox & Dashboard", Icon = "home", Route = "/dashboard", BadgeCount = 4, Category = "Main", Order = 1, IsActive = true });
                siteNavLinks.Add(new SiteNavLinkDto { Id = "nav-assets", Label = "Asset Operations", Icon = "box", Route = "/assets", BadgeCount = 12, Category = "Management", Order = 2, IsActive = true });
                siteNavLinks.Add(new SiteNavLinkDto { Id = "nav-compliance", Label = "Compliance & Safety", Icon = "check-circle", Route = "/compliance", Category = "Management", Order = 3, IsActive = true });
                siteNavLinks.Add(new SiteNavLinkDto { Id = "nav-audits", Label = "Audit Archive", Icon = "file-text", Route = "/audits", Category = "Archive", Order = 4, IsActive = true });
                siteNavLinks.Add(new SiteNavLinkDto { Id = "nav-analytics", Label = "Reports & Analytics", Icon = "bar-chart", Route = "/analytics", Category = "Archive", Order = 5, IsActive = true });
            }

            List<DashboardFormMetadataDto> dashboardForms = GetDashboardForms();

            UserBootstrapDto bootstrap = new UserBootstrapDto
            {
                UserId = "USR-2026-4402",
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
                return TypedResults.NotFound(new { message = $"Page '{pageKey}' not found." });
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
                return TypedResults.NotFound(new { message = $"Form schema '{formKey}' not found." });
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

        private static List<DashboardFormMetadataDto> GetDashboardForms()
        {
            return new List<DashboardFormMetadataDto>
            {
                new DashboardFormMetadataDto
                {
                    FormId = "widget-active-assets",
                    FormType = "widget",
                    Title = "Active Enterprise Assets",
                    Caption = "Cataloged Hardware & Software",
                    GridCols = 4,
                    WidgetConfig = new WidgetConfigDto
                    {
                        WidgetType = "kpi",
                        MetricValue = "1,428",
                        MetricTrend = "+8.2% vs last month",
                        TrendDirection = "up",
                        AccentColor = "#3b82f6",
                        Icon = "box"
                    }
                },
                new DashboardFormMetadataDto
                {
                    FormId = "widget-pending-maintenance",
                    FormType = "widget",
                    Title = "Pending Maintenance SLA",
                    Caption = "Overdue Service Requests",
                    GridCols = 4,
                    WidgetConfig = new WidgetConfigDto
                    {
                        WidgetType = "kpi",
                        MetricValue = "14 Assets",
                        MetricTrend = "-3.1% this week",
                        TrendDirection = "down",
                        AccentColor = "#f59e0b",
                        Icon = "alert-triangle"
                    }
                },
                new DashboardFormMetadataDto
                {
                    FormId = "widget-compliance-score",
                    FormType = "widget",
                    Title = "Field Compliance Score",
                    Caption = "Quarterly Safety Verification",
                    GridCols = 4,
                    WidgetConfig = new WidgetConfigDto
                    {
                        WidgetType = "kpi",
                        MetricValue = "98.4%",
                        MetricTrend = "+1.2% this quarter",
                        TrendDirection = "up",
                        AccentColor = "#10b981",
                        Icon = "shield-check"
                    }
                },
                new DashboardFormMetadataDto
                {
                    FormId = "search-asset-filter",
                    FormType = "search",
                    Title = "Asset Search & Criteria Filter",
                    Caption = "Search active inventory and audit history",
                    Description = "Filter items across category, warranty level, and critical tags",
                    IsEditable = true,
                    GridCols = 12,
                    SearchConfig = new SearchConfigDto
                    {
                        TargetGridId = "grid-inbox-items",
                        AutoSubmitOnReset = true,
                        SubmitButtonLabel = "Apply Filters"
                    },
                    Fields = new List<FormFieldDto>
                    {
                        new FormFieldDto
                        {
                            Key = "searchKeyword",
                            Label = "Search Keyword",
                            Type = "text",
                            Placeholder = "Enter Tag (e.g. AST-2026) or device name...",
                            GridCols = 4
                        },
                        new FormFieldDto
                        {
                            Key = "categoryFilter",
                            Label = "Asset Category",
                            Type = "select",
                            GridCols = 4,
                            Options = new List<SelectOptionDto>
                            {
                                new SelectOptionDto { Label = "All Categories", Value = "" },
                                new SelectOptionDto { Label = "Hardware & Workstations", Value = "Hardware" },
                                new SelectOptionDto { Label = "Networking Equipment", Value = "Networking" },
                                new SelectOptionDto { Label = "Mobile Devices", Value = "Mobile" },
                                new SelectOptionDto { Label = "Software Licenses", Value = "Software" }
                            }
                        },
                        new FormFieldDto
                        {
                            Key = "criticalOnly",
                            Label = "Critical SLA Only",
                            Type = "toggle",
                            GridCols = 4,
                            HelpText = "Show high priority infrastructure items"
                        }
                    }
                },
                new DashboardFormMetadataDto
                {
                    FormId = "grid-inbox-items",
                    FormType = "grid",
                    Title = "Inbox Action Items & Asset Maintenance Queue",
                    Caption = "Recent alerts, inspection assignments, and work orders",
                    GridCols = 12,
                    GridConfig = new GridConfigDto
                    {
                        PageSize = 5,
                        AllowSorting = true,
                        AllowPaging = true,
                        Rows = new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object>
                            {
                                { "id", "TASK-9901" },
                                { "assetTag", "AST-2026-9901" },
                                { "name", "Dell XPS 15 Workstation" },
                                { "type", "Inspection Audit" },
                                { "priority", "High" },
                                { "dueDate", "2026-08-25" },
                                { "status", "Pending Approval" }
                            },
                            new Dictionary<string, object>
                            {
                                { "id", "TASK-8812" },
                                { "assetTag", "AST-2026-4402" },
                                { "name", "Cisco Catalyst 9300 Switch" },
                                { "type", "Firmware Patch" },
                                { "priority", "Critical" },
                                { "dueDate", "2026-08-24" },
                                { "status", "Scheduled" }
                            },
                            new Dictionary<string, object>
                            {
                                { "id", "TASK-7734" },
                                { "assetTag", "AST-2026-1120" },
                                { "name", "Apple MacBook Pro M3" },
                                { "type", "Warranty Renewal" },
                                { "priority", "Medium" },
                                { "dueDate", "2026-08-30" },
                                { "status", "In Review" }
                            },
                            new Dictionary<string, object>
                            {
                                { "id", "TASK-6621" },
                                { "assetTag", "AST-2026-8810" },
                                { "name", "Lenovo ThinkPad P16" },
                                { "type", "Safety Inspection" },
                                { "priority", "Low" },
                                { "dueDate", "2026-09-02" },
                                { "status", "Completed" }
                            }
                        }
                    }
                },
                new DashboardFormMetadataDto
                {
                    FormId = "detail-selected-asset",
                    FormType = "detail",
                    Title = "Asset Inspection Record (Read-Only)",
                    Caption = "Verified Inspection Report AUD-2026-8810",
                    Description = "Archived safety inspection data.",
                    IsEditable = false,
                    LabelPosition = "top",
                    GridCols = 12,
                    Fields = new List<FormFieldDto>
                    {
                        new FormFieldDto
                        {
                            Key = "inspectorName",
                            Label = "Inspector Name",
                            Type = "text",
                            DefaultValue = "Alex Rivera (Tech ID: 4402)",
                            GridCols = 6,
                            Readonly = true
                        },
                        new FormFieldDto
                        {
                            Key = "inspectionDate",
                            Label = "Inspection Date",
                            Type = "date",
                            DefaultValue = "2026-08-23",
                            GridCols = 6,
                            Readonly = true
                        },
                        new FormFieldDto
                        {
                            Key = "conditionGrade",
                            Label = "Physical Condition Grade",
                            Type = "radio",
                            DefaultValue = "Excellent",
                            GridCols = 6,
                            Readonly = true,
                            Options = new List<SelectOptionDto>
                            {
                                new SelectOptionDto { Label = "Excellent (Like New)", Value = "Excellent" },
                                new SelectOptionDto { Label = "Good (Normal Wear)", Value = "Good" },
                                new SelectOptionDto { Label = "Fair (Needs Repair)", Value = "Fair" }
                            }
                        },
                        new FormFieldDto
                        {
                            Key = "passedSecurityCheck",
                            Label = "Passed Cybersecurity Compliance Audit",
                            Type = "toggle",
                            DefaultValue = true,
                            GridCols = 6,
                            Readonly = true
                        },
                        new FormFieldDto
                        {
                            Key = "defectDetails",
                            Label = "Defects or Observations",
                            Type = "textarea",
                            DefaultValue = "All diagnostics passed cleanly. Thermal vents cleaned.",
                            GridCols = 12,
                            Readonly = true
                        }
                    }
                }
            };
        }
    }
}
