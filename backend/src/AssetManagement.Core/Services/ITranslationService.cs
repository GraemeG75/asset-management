using AssetManagement.Core.Dtos;

namespace AssetManagement.Core.Services
{
    public interface ITranslationService
    {
        TranslationResponseDto GetPublicTranslations(string? culture = "en");
        TranslationResponseDto GetAuthenticatedTranslations(string? culture = "en");
    }
}
