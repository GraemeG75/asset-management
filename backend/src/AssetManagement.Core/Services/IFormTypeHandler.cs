using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;

namespace AssetManagement.Core.Services
{
    public interface IFormTypeHandler
    {
        string FormType { get; }
        Task<FormSubmissionResultDto> HandleSubmissionAsync(FormSubmissionContext context);
    }
}
