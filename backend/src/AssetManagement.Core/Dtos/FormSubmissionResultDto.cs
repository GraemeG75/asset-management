using System.Collections.Generic;

namespace AssetManagement.Core.Dtos
{
    public class FormFieldErrorDto
    {
        public string FieldKey { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class FormSubmissionResultDto
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public string? RecordId { get; set; }
        public string FormKey { get; set; } = string.Empty;
        public string FormType { get; set; } = "standard";
        public Dictionary<string, object?>? Data { get; set; }
        public List<FormFieldErrorDto>? FieldErrors { get; set; }
    }
}
