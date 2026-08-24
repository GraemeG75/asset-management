using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;

namespace AssetManagement.Infrastructure.Services.FormHandlers
{
    public class DetailFormHandler : IFormTypeHandler
    {
        public string FormType => "detail";

        public Task<FormSubmissionResultDto> HandleSubmissionAsync(FormSubmissionContext context)
        {
            FormSubmissionDto submission = context.Submission;
            string targetId = submission.RecordId ?? Guid.NewGuid().ToString();
            string formTitle = !string.IsNullOrWhiteSpace(context.FormMetadata?.Title)
                ? context.FormMetadata.Title
                : (!string.IsNullOrWhiteSpace(context.FormMetadata?.Caption) ? context.FormMetadata.Caption : submission.FormKey);

            return Task.FromResult(new FormSubmissionResultDto
            {
                Success = true,
                Message = $"Form '{formTitle}' record '{targetId}' updated successfully",
                RecordId = targetId,
                FormKey = submission.FormKey,
                FormType = FormType,
                Data = new Dictionary<string, object?>(submission.FieldValues)
            });
        }
    }
}
