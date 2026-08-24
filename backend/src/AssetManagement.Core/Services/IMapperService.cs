using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;

namespace AssetManagement.Core.Services
{
    public interface IMapperService
    {
        Task<Dictionary<string, object?>> LoadFormDataAsync(string formKey, string? recordId = null);
        Task<List<FormFieldErrorDto>> ValidateFormDataAsync(FormSubmissionDto submission);
        Task<FormSubmissionResultDto> SaveFormDataAsync(FormSubmissionDto submission);
    }
}
