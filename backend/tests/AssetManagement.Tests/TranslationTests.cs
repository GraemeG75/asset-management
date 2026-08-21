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
    }
}
