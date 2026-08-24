using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;

namespace AssetManagement.Infrastructure.Services.FormHandlers
{
    public class WidgetFormHandler : IFormTypeHandler
    {
        public string FormType => "widget";

        public Task<FormSubmissionResultDto> HandleSubmissionAsync(FormSubmissionContext context)
        {
            FormSubmissionDto submission = context.Submission;
            string formTitle = !string.IsNullOrWhiteSpace(context.FormMetadata?.Title)
                ? context.FormMetadata.Title
                : (!string.IsNullOrWhiteSpace(context.FormMetadata?.Caption) ? context.FormMetadata.Caption : submission.FormKey);

            return Task.FromResult(new FormSubmissionResultDto
            {
                Success = true,
                Message = $"Widget '{formTitle}' action executed successfully",
                RecordId = submission.RecordId,
                FormKey = submission.FormKey,
                FormType = FormType,
                Data = new Dictionary<string, object?>(submission.FieldValues)
                {
                    ["executedAt"] = System.DateTime.UtcNow
                }
            });
        }
    }
}
