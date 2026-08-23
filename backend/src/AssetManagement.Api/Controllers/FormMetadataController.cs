using AssetManagement.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers;

[ApiController]
[Route("api/form-metadata")]
public class FormMetadataController : ControllerBase
{
    [HttpGet("pages")]
    public ActionResult<List<object>> GetAvailablePages()
    {
        var pages = new List<object>
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
        var pageInfo = pageId.ToLowerInvariant() switch
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
        var schema = formId.ToLowerInvariant() switch
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
}
