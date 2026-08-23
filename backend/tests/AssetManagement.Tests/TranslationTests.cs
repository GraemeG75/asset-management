using System.Collections.Generic;
using AssetManagement.Api.Controllers;
using AssetManagement.Core.Dtos;
using AssetManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace AssetManagement.Tests
{
    public class TranslationTests
    {
        [Fact]
        public void GetPublicTranslations_ReturnsPublicDictionaryForEnglish()
        {
            TranslationService translationService = new TranslationService();
            TranslationsController controller = new TranslationsController(translationService);

            IResult result = controller.GetPublicTranslations("en");

            Ok<TranslationResponseDto> okResult = Assert.IsType<Ok<TranslationResponseDto>>(result);
            TranslationResponseDto? response = okResult.Value;

            Assert.NotNull(response);
            Assert.Equal("en", response.Culture);
            Assert.True(response.Translations.ContainsKey("LOGIN_TITLE"));
            Assert.Equal("Sign in to AssetPulse", response.Translations["LOGIN_TITLE"]);
        }

        [Theory]
        [InlineData("es", "Iniciar sesión en AssetPulse")]
        [InlineData("fr", "Connexion à AssetPulse")]
        [InlineData("de", "Anmelden bei AssetPulse")]
        public void GetPublicTranslations_ReturnsLocalizedResourceFromResx(string culture, string expectedTitle)
        {
            TranslationService translationService = new TranslationService();
            TranslationsController controller = new TranslationsController(translationService);

            IResult result = controller.GetPublicTranslations(culture);

            Ok<TranslationResponseDto> okResult = Assert.IsType<Ok<TranslationResponseDto>>(result);
            TranslationResponseDto? response = okResult.Value;

            Assert.NotNull(response);
            Assert.Equal(culture, response.Culture);
            Assert.True(response.Translations.ContainsKey("LOGIN_TITLE"));
            Assert.Equal(expectedTitle, response.Translations["LOGIN_TITLE"]);
        }

        [Fact]
        public void GetAuthenticatedTranslations_ReturnsAuthenticatedDictionary()
        {
            TranslationService translationService = new TranslationService();
            TranslationsController controller = new TranslationsController(translationService);

            IResult result = controller.GetAuthenticatedTranslations("en");

            Ok<TranslationResponseDto> okResult = Assert.IsType<Ok<TranslationResponseDto>>(result);
            TranslationResponseDto? response = okResult.Value;

            Assert.NotNull(response);
            Assert.Equal("en", response.Culture);
            Assert.True(response.Translations.ContainsKey("NAV_DASHBOARD"));
            Assert.Equal("Dashboard", response.Translations["NAV_DASHBOARD"]);
        }

        [Theory]
        [InlineData("en-US", "User profile not found.")]
        [InlineData("es-ES", "Perfil de usuario no encontrado.")]
        [InlineData("fr-FR", "Profil utilisateur non trouvé.")]
        [InlineData("de-DE", "Benutzerprofil nicht gefunden.")]
        public void GetString_ReturnsLocalizedErrorMessageForUserLocale(string culture, string expectedMessage)
        {
            TranslationService translationService = new TranslationService();
            string localizedMsg = translationService.GetString("ERR_USER_NOT_FOUND", culture);
            Assert.Equal(expectedMessage, localizedMsg);
        }

        [Theory]
        [InlineData("en-US", "dashboard", "Page 'dashboard' was not found.")]
        [InlineData("es-ES", "dashboard", "No se encontró la página 'dashboard'.")]
        [InlineData("fr-FR", "dashboard", "La page 'dashboard' est introuvable.")]
        [InlineData("de-DE", "dashboard", "Seite 'dashboard' wurde nicht gefunden.")]
        public void GetString_FormatsArgumentsForLocalizedErrorMessages(string culture, string pageKey, string expectedMessage)
        {
            TranslationService translationService = new TranslationService();
            string localizedMsg = translationService.GetString("ERR_PAGE_NOT_FOUND", culture, pageKey);
            Assert.Equal(expectedMessage, localizedMsg);
        }
    }
}
