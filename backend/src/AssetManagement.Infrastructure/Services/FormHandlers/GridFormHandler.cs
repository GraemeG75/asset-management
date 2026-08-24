using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;

namespace AssetManagement.Infrastructure.Services.FormHandlers
{
    public class GridFormHandler : IFormTypeHandler
    {
        public string FormType => "grid";

        public Task<FormSubmissionResultDto> HandleSubmissionAsync(FormSubmissionContext context)
        {
            FormSubmissionDto submission = context.Submission;
            string action = submission.Action?.ToLowerInvariant() ?? "update";
            string recordId = submission.RecordId ?? Guid.NewGuid().ToString();
            string formTitle = !string.IsNullOrWhiteSpace(context.FormMetadata?.Title)
                ? context.FormMetadata.Title
                : (!string.IsNullOrWhiteSpace(context.FormMetadata?.Caption) ? context.FormMetadata.Caption : submission.FormKey);

            string message = action switch
            {
                "create" => $"Grid '{formTitle}' record '{recordId}' created successfully",
                "delete" => $"Grid '{formTitle}' record '{recordId}' deleted successfully",
                _ => $"Grid '{formTitle}' record '{recordId}' updated successfully"
            };

            Dictionary<string, object?> responseData = new Dictionary<string, object?>(submission.FieldValues)
            {
                ["actionExecuted"] = action,
                ["gridRecordId"] = recordId
            };

            return Task.FromResult(new FormSubmissionResultDto
            {
                Success = true,
                Message = message,
                RecordId = recordId,
                FormKey = submission.FormKey,
                FormType = FormType,
                Data = responseData
            });
        }
    }
}
