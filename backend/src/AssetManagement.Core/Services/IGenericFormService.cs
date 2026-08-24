using System.Threading.Tasks;
using AssetManagement.Core.Dtos;

namespace AssetManagement.Core.Services
{
    public interface IGenericFormService
    {
        Task<FormSubmissionResultDto> SubmitFormAsync(FormSubmissionDto submission, string? userId = null, string? locale = null);
    }
}
