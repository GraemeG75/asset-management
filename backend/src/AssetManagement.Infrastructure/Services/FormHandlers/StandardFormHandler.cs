using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;

namespace AssetManagement.Infrastructure.Services.FormHandlers
{
    public class StandardFormHandler : IFormTypeHandler
    {
        public string FormType => "standard";

        private readonly IMapperService _mapperService;

        public StandardFormHandler(IMapperService mapperService)
        {
            _mapperService = mapperService;
        }

        public async Task<FormSubmissionResultDto> HandleSubmissionAsync(FormSubmissionContext context)
        {
            return await _mapperService.SaveFormDataAsync(context.Submission);
        }
    }
}
