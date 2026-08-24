using System.Collections.Generic;
using AssetManagement.Core.Dtos;

namespace AssetManagement.Core.Models
{
    public class FormSubmissionContext
    {
        public required FormSubmissionDto Submission { get; set; }
        public required string UserId { get; set; }
        public string Locale { get; set; } = "en-US";
        public XFormEntity? FormMetadata { get; set; }
        public IEnumerable<XMapperFlavorFieldEntity>? FlavorFields { get; set; }
    }
}
