using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;

namespace AssetManagement.Infrastructure.Services.FormHandlers
{
    public class SearchFormHandler : IFormTypeHandler
    {
        public string FormType => "search";

        public Task<FormSubmissionResultDto> HandleSubmissionAsync(FormSubmissionContext context)
        {
            FormSubmissionDto submission = context.Submission;
            string formTitle = !string.IsNullOrWhiteSpace(context.FormMetadata?.Title)
                ? context.FormMetadata.Title
                : (!string.IsNullOrWhiteSpace(context.FormMetadata?.Caption) ? context.FormMetadata.Caption : submission.FormKey);

            Dictionary<string, object?> activeCriteria = new Dictionary<string, object?>();
            foreach (KeyValuePair<string, object?> kvp in submission.FieldValues)
            {
                if (kvp.Value != null && !string.IsNullOrWhiteSpace(kvp.Value.ToString()))
                {
                    activeCriteria[kvp.Key] = kvp.Value;
                }
            }

            return Task.FromResult(new FormSubmissionResultDto
            {
                Success = true,
                Message = $"Search '{formTitle}' criteria processed ({activeCriteria.Count} active filter(s))",
                RecordId = null,
                FormKey = submission.FormKey,
                FormType = FormType,
                Data = new Dictionary<string, object?>
                {
                    ["activeFilters"] = activeCriteria,
                    ["appliedCount"] = activeCriteria.Count
                }
            });
        }
    }
}
