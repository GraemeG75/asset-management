using AssetManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    [ApiController]
    [Route("api/form-metadata")]
    public class FormMetadataController : ControllerBase
    {
        /// <summary>
        /// Retrieves user bootstrap metadata upon login (Profile nav, Site nav, Inbox, Dashboard form types)
        /// </summary>
        /// <returns>UserBootstrapDto containing navigation links, inbox count, and dashboard form schemas</returns>
        [HttpGet("user-bootstrap")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserBootstrapDto))]
        public IResult GetUserBootstrap()
        {
            UserBootstrapDto bootstrap = GetUserBootstrapData();
            return TypedResults.Ok(bootstrap);
        }

        [HttpGet("pages")]
        public ActionResult<List<object>> GetAvailablePages()
    {
        List<object> pages = new List<object>
        {
            new { pageId = "asset-operations", title = "Asset Operations Page", description = "Contains asset registration and maintenance forms" },
            new { pageId = "user-settings", title = "User Account & Profile Page", description = "Contains user settings and notification forms" },
            new { pageId = "audit-reports", title = "Compliance & Audit Page", description = "Contains read-only asset audit summary forms" }
        };
        return Ok(pages);
    }

    [HttpGet("pages/{pageId}")]
    public ActionResult<PageInfoDto> GetPageInfo(string pageId)
    {
        PageInfoDto? pageInfo = pageId.ToLowerInvariant() switch
        {
            "asset-operations" => new PageInfoDto
            {
                PageId = "asset-operations",
                Title = "Asset Operations Workspace",
                Description = "Catalog physical assets and request scheduled maintenance",
                Forms = new List<PageFormSummaryDto>
                {
                    new PageFormSummaryDto { FormId = "asset-create", Caption = "New Asset Registration Form", Description = "Form to register a new IT or enterprise asset" }
                }
            },
            "user-settings" => new PageInfoDto
            {
                PageId = "user-settings",
                Title = "User Account & Preference Center",
                Description = "Manage personal details, department assignment, and notification options",
                Forms = new List<PageFormSummaryDto>
                {
                    new PageFormSummaryDto { FormId = "user-profile", Caption = "User Profile & Preferences Form", Description = "Form to update user settings and communication preferences" }
                }
            },
            "audit-reports" => new PageInfoDto
            {
                PageId = "audit-reports",
                Title = "Compliance Audit Archive",
                Description = "Review historical asset inspection and condition audit records",
                Forms = new List<PageFormSummaryDto>
                {
                    new PageFormSummaryDto { FormId = "asset-inspection", Caption = "Asset Condition Inspection Audit", Description = "Read-only summary view of field asset safety inspection" }
                }
            },
            _ => null
        };

        if (pageInfo == null)
        {
            return NotFound(new { message = $"Page '{pageId}' not found." });
        }

        return Ok(pageInfo);
    }

    [HttpGet("{formId}")]
    public ActionResult<FormSchemaDto> GetFormSchema(string formId)
    {
        FormSchemaDto? schema = formId.ToLowerInvariant() switch
        {
            "asset-create" => GetAssetCreateFormSchema(),
            "user-profile" => GetUserProfileFormSchema(),
            "asset-inspection" => GetAssetInspectionFormSchema(),
            _ => null
        };

        if (schema == null)
        {
            return NotFound(new { message = $"Form schema '{formId}' not found." });
        }

        return Ok(schema);
    }

    private static FormSchemaDto GetAssetCreateFormSchema()
    {
        return new FormSchemaDto
        {
            Id = "asset-create",
            Caption = "New Asset Registration Form",
            Title = "Asset Registration",
            Description = "Enter asset metadata for cataloging in the asset management system.",
            FormInfo = "Please ensure asset barcode tag complies with AST-YYYY-NNNN standard.",
            IsEditable = true,
            SubmitButtonText = "Save Asset",
            ShowResetButton = true,
            Fields = new List<FormFieldDto>
            {
                new FormFieldDto
                {
                    Key = "assetTag",
                    Label = "Asset Tag Number",
                    Type = "text",
                    Placeholder = "e.g. AST-2026-9901",
                    HelpText = "Unique barcode identifier attached to physical asset",
                    GridCols = 6,
                    Validators = new List<FieldValidatorDto>
                    {
                        new FieldValidatorDto { Type = "required", Message = "Asset Tag is required" },
                        new FieldValidatorDto { Type = "pattern", Value = "^[A-Z]{3}-\\d{4}-\\d{4}$", Message = "Format must match AST-YYYY-NNNN" }
                    }
                },
                new FormFieldDto
                {
                    Key = "name",
                    Label = "Asset Name",
                    Type = "text",
                    Placeholder = "Dell XPS 15 9530 Workstation",
                    GridCols = 6,
                    Validators = new List<FieldValidatorDto>
                    {
                        new FieldValidatorDto { Type = "required", Message = "Asset Name is required" },
                        new FieldValidatorDto { Type = "minLength", Value = 3, Message = "Name must be at least 3 characters" }
                    }
                },
                new FormFieldDto
                {
                    Key = "category",
                    Label = "Category",
                    Type = "select",
                    GridCols = 6,
                    DefaultValue = "Hardware",
                    Options = new List<SelectOptionDto>
                    {
                        new SelectOptionDto { Label = "Hardware & Workstations", Value = "Hardware" },
                        new SelectOptionDto { Label = "Networking Equipment", Value = "Networking" },
                        new SelectOptionDto { Label = "Mobile Devices", Value = "Mobile" },
                        new SelectOptionDto { Label = "Software & Licenses", Value = "Software" },
                        new SelectOptionDto { Label = "Office Furniture", Value = "Furniture" }
                    },
                    Validators = new List<FieldValidatorDto>
                    {
                        new FieldValidatorDto { Type = "required", Message = "Please select a category" }
                    }
                },
                new FormFieldDto
                {
                    Key = "purchasePrice",
                    Label = "Purchase Price ($)",
                    Type = "number",
                    Placeholder = "2499.99",
                    GridCols = 6,
                    Validators = new List<FieldValidatorDto>
                    {
                        new FieldValidatorDto { Type = "required", Message = "Purchase price is required" },
                        new FieldValidatorDto { Type = "min", Value = 0, Message = "Price must be greater than or equal to 0" }
                    }
                },
                new FormFieldDto
                {
                    Key = "purchaseDate",
                    Label = "Purchase Date",
                    Type = "date",
                    GridCols = 6,
                    DefaultValue = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    Validators = new List<FieldValidatorDto>
                    {
                        new FieldValidatorDto { Type = "required", Message = "Purchase date is required" }
                    }
                },
                new FormFieldDto
                {
                    Key = "warrantyStatus",
                    Label = "Warranty Level",
                    Type = "radio",
                    GridCols = 6,
                    DefaultValue = "Standard",
                    Options = new List<SelectOptionDto>
                    {
                        new SelectOptionDto { Label = "Basic (1 Year)", Value = "Basic" },
                        new SelectOptionDto { Label = "Standard (3 Years)", Value = "Standard" },
                        new SelectOptionDto { Label = "Enterprise Pro (5 Years)", Value = "Enterprise" }
                    }
                },
                new FormFieldDto
                {
                    Key = "isCriticalAsset",
                    Label = "Critical Infrastructure Asset",
                    Type = "toggle",
                    GridCols = 6,
                    DefaultValue = false,
                    HelpText = "Flag asset for 24/7 priority SLA monitoring"
                },
                new FormFieldDto
                {
                    Key = "requiresMaintenance",
                    Label = "Scheduled Maintenance Required",
                    Type = "checkbox",
                    GridCols = 6,
                    DefaultValue = true
                },
                new FormFieldDto
                {
                    Key = "notes",
                    Label = "Asset Notes & Description",
                    Type = "textarea",
                    Placeholder = "Include serial numbers, location rack position, software image installed...",
                    GridCols = 12
                }
            }
        };
    }

    private static FormSchemaDto GetUserProfileFormSchema()
    {
        return new FormSchemaDto
        {
            Id = "user-profile",
            Caption = "User Profile & Preferences Form",
            Title = "User Profile & Preferences",
            Description = "Manage personal details, department assignment, and notification toggles.",
            FormInfo = "User changes will take effect upon saving.",
            IsEditable = true,
            SubmitButtonText = "Update Profile",
            ShowResetButton = true,
            Fields = new List<FormFieldDto>
            {
                new FormFieldDto
                {
                    Key = "fullName",
                    Label = "Full Name",
                    Type = "text",
                    DefaultValue = "Sarah Connor",
                    GridCols = 6,
                    Validators = new List<FieldValidatorDto>
                    {
                        new FieldValidatorDto { Type = "required", Message = "Full name is required" }
                    }
                },
                new FormFieldDto
                {
                    Key = "email",
                    Label = "Email Address",
                    Type = "email",
                    DefaultValue = "s.connor@enterprise.com",
                    GridCols = 6,
                    Validators = new List<FieldValidatorDto>
                    {
                        new FieldValidatorDto { Type = "required", Message = "Email is required" },
                        new FieldValidatorDto { Type = "email", Message = "Please enter a valid email address" }
                    }
                },
                new FormFieldDto
                {
                    Key = "department",
                    Label = "Department",
                    Type = "select",
                    GridCols = 6,
                    DefaultValue = "IT Operations",
                    Options = new List<SelectOptionDto>
                    {
                        new SelectOptionDto { Label = "IT Operations", Value = "IT Operations" },
                        new SelectOptionDto { Label = "Facilities & Logistics", Value = "Facilities" },
                        new SelectOptionDto { Label = "Finance & Procurement", Value = "Finance" },
                        new SelectOptionDto { Label = "Engineering", Value = "Engineering" }
                    }
                },
                new FormFieldDto
                {
                    Key = "emailNotifications",
                    Label = "Email Alerts",
                    Type = "toggle",
                    DefaultValue = true,
                    GridCols = 6,
                    HelpText = "Receive email alerts when assets assigned to your team require audit"
                },
                new FormFieldDto
                {
                    Key = "bio",
                    Label = "Bio / Department Notes",
                    Type = "textarea",
                    Placeholder = "Add any special equipment handling permissions...",
                    GridCols = 12
                }
            }
        };
    }

    private static FormSchemaDto GetAssetInspectionFormSchema()
    {
        return new FormSchemaDto
        {
            Id = "asset-inspection",
            Caption = "Asset Condition Inspection Audit",
            Title = "Asset Condition Inspection Audit (Read-Only Archive)",
            Description = "Completed physical asset audit report. In read-only mode, all fields are locked and labels appear on top.",
            FormInfo = "Record ID: AUD-2026-8810 • Audit Status: Closed & Approved",
            IsEditable = false,
            SubmitButtonText = "Acknowledge Report",
            ShowResetButton = false,
            Fields = new List<FormFieldDto>
            {
                new FormFieldDto
                {
                    Key = "inspectorName",
                    Label = "Inspector Name",
                    Type = "text",
                    DefaultValue = "Alex Rivera (Tech ID: 4402)",
                    GridCols = 6
                },
                new FormFieldDto
                {
                    Key = "inspectionDate",
                    Label = "Inspection Date",
                    Type = "date",
                    GridCols = 6,
                    DefaultValue = DateTime.UtcNow.ToString("yyyy-MM-dd")
                },
                new FormFieldDto
                {
                    Key = "physicalCondition",
                    Label = "Physical Condition Grade",
                    Type = "radio",
                    GridCols = 6,
                    DefaultValue = "Excellent",
                    Options = new List<SelectOptionDto>
                    {
                        new SelectOptionDto { Label = "Excellent (Like New)", Value = "Excellent" },
                        new SelectOptionDto { Label = "Good (Normal Wear)", Value = "Good" },
                        new SelectOptionDto { Label = "Fair (Needs Minor Repair)", Value = "Fair" },
                        new SelectOptionDto { Label = "Poor (Requires Replacement)", Value = "Poor" }
                    }
                },
                new FormFieldDto
                {
                    Key = "passedSecurityCheck",
                    Label = "Passed Cybersecurity Compliance Audit",
                    Type = "toggle",
                    GridCols = 6,
                    DefaultValue = true
                },
                new FormFieldDto
                {
                    Key = "defectDetails",
                    Label = "Defects or Observations",
                    Type = "textarea",
                    DefaultValue = "All diagnostics passed. Battery health 98%. Cleaned thermal vents.",
                    GridCols = 12
                }
            }
        };
    }

    private static UserBootstrapDto GetUserBootstrapData()
    {
        List<ProfileNavLinkDto> profileNavLinks = new List<ProfileNavLinkDto>
        {
            new ProfileNavLinkDto { Id = "profile-settings", Label = "My Profile", Icon = "user", Url = "/profile", Order = 1, IsActive = true },
            new ProfileNavLinkDto { Id = "profile-security", Label = "Security & Credentials", Icon = "shield", Url = "/profile/security", Order = 2, IsActive = true },
            new ProfileNavLinkDto { Id = "profile-preferences", Label = "Language & Preferences", Icon = "sliders", Url = "/profile/preferences", Order = 3, IsActive = true },
            new ProfileNavLinkDto { Id = "profile-help", Label = "Help & Documentation", Icon = "help-circle", Url = "/help", Order = 4, IsActive = true },
            new ProfileNavLinkDto { Id = "profile-logout", Label = "Log Out", Icon = "log-out", Url = "/logout", Order = 5, IsActive = true }
        };

        List<SiteNavLinkDto> siteNavLinks = new List<SiteNavLinkDto>
        {
            new SiteNavLinkDto { Id = "nav-dashboard", Label = "Inbox & Dashboard", Icon = "home", Route = "/dashboard", BadgeCount = 4, Category = "Main", Order = 1, IsActive = true },
            new SiteNavLinkDto { Id = "nav-assets", Label = "Asset Operations", Icon = "box", Route = "/assets", BadgeCount = 12, Category = "Management", Order = 2, IsActive = true },
            new SiteNavLinkDto { Id = "nav-compliance", Label = "Compliance & Safety", Icon = "check-circle", Route = "/compliance", Category = "Management", Order = 3, IsActive = true },
            new SiteNavLinkDto { Id = "nav-audits", Label = "Audit Archive", Icon = "file-text", Route = "/audits", Category = "Archive", Order = 4, IsActive = true },
            new SiteNavLinkDto { Id = "nav-analytics", Label = "Reports & Analytics", Icon = "bar-chart", Route = "/analytics", Category = "Archive", Order = 5, IsActive = true }
        };

        List<DashboardFormMetadataDto> dashboardForms = new List<DashboardFormMetadataDto>
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

        return new UserBootstrapDto
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
    }
}
}

