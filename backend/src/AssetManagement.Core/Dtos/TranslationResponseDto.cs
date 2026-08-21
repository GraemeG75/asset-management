using System.Collections.Generic;

namespace AssetManagement.Core.Dtos
{
    public record TranslationResponseDto(
        string Culture,
        Dictionary<string, string> Translations
    );
}
