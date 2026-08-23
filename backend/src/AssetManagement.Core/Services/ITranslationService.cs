using AssetManagement.Core.Dtos;

namespace AssetManagement.Core.Services
{
    public interface ITranslationService
    {
        TranslationResponseDto GetPublicTranslations(string? culture = "en");
        TranslationResponseDto GetAuthenticatedTranslations(string? culture = "en");
        string GetString(string key, string? culture = "en-US", params object[] args);
    }
}
