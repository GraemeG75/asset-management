using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Services;

namespace AssetManagement.Infrastructure.Services
{
    /// <summary>
    /// Serves localized translation dictionaries and localized strings from compiled .resx resource files
    /// </summary>
    public class TranslationService : ITranslationService
    {
        private readonly ResourceManager _resourceManager;

        private static readonly HashSet<string> PublicKeys = new HashSet<string>
        {
            "APP_TITLE", "LOGIN_TITLE", "LOGIN_SUBTITLE", "EMAIL_LABEL", "EMAIL_PLACEHOLDER",
            "EMAIL_REQUIRED", "EMAIL_INVALID", "PASSWORD_LABEL", "PASSWORD_PLACEHOLDER",
            "PASSWORD_REQUIRED", "PASSWORD_MINLENGTH", "REMEMBER_ME", "SIGN_IN_BTN",
            "AUTHENTICATING", "OR_SIGN_IN_WITH", "SSO_GOOGLE", "SSO_MICROSOFT",
            "SSO_GITHUB", "NAV_BRAND", "DEMO_CREDENTIALS", "LANGUAGE_SELECTOR"
        };

        private static readonly HashSet<string> AuthenticatedKeys = new HashSet<string>
        {
            "NAV_DASHBOARD", "NAV_ASSETS", "NAV_CATEGORIES", "NAV_REPORTS", "NAV_SETTINGS",
            "NAV_LOGOUT", "USER_PROFILE", "ROLE_ADMIN", "ROLE_MANAGER", "ROLE_USER",
            "WELCOME_BACK", "TOTAL_ASSETS", "ACTIVE_ASSETS", "MAINTENANCE_DUE", "SYSTEM_HEALTH"
        };

        public TranslationService()
        {
            _resourceManager = new ResourceManager("AssetManagement.Infrastructure.Resources.Translations", typeof(TranslationService).Assembly);
        }

        /// <summary>
        /// Retrieves unauthenticated public translations from .resx resource files based on CultureInfo
        /// </summary>
        public TranslationResponseDto GetPublicTranslations(string? culture = "en")
        {
            CultureInfo cultureInfo = GetCultureInfo(culture);
            Dictionary<string, string> dictionary = GetDictionaryForKeys(cultureInfo, PublicKeys);
            return new TranslationResponseDto(cultureInfo.TwoLetterISOLanguageName, dictionary);
        }

        /// <summary>
        /// Retrieves authenticated translations from .resx resource files based on CultureInfo
        /// </summary>
        public TranslationResponseDto GetAuthenticatedTranslations(string? culture = "en")
        {
            CultureInfo cultureInfo = GetCultureInfo(culture);
            Dictionary<string, string> dictionary = GetDictionaryForKeys(cultureInfo, AuthenticatedKeys);
            return new TranslationResponseDto(cultureInfo.TwoLetterISOLanguageName, dictionary);
        }

        /// <summary>
        /// Retrieves formatted localized string from .resx resource files based on key and culture
        /// </summary>
        public string GetString(string key, string? culture = "en-US", params object[] args)
        {
            CultureInfo cultureInfo = GetCultureInfo(culture);
            string? value = _resourceManager.GetString(key, cultureInfo);

            if (string.IsNullOrEmpty(value))
            {
                value = key;
            }

            if (args != null && args.Length > 0)
            {
                try
                {
                    return string.Format(cultureInfo, value, args);
                }
                catch
                {
                    return value;
                }
            }

            return value;
        }

        private Dictionary<string, string> GetDictionaryForKeys(CultureInfo cultureInfo, HashSet<string> targetKeys)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (string key in targetKeys)
            {
                string? value = _resourceManager.GetString(key, cultureInfo);
                if (!string.IsNullOrEmpty(value))
                {
                    result[key] = value;
                }
            }

            return result;
        }

        private static CultureInfo GetCultureInfo(string? culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return CultureInfo.GetCultureInfo("en-US");
            }

            try
            {
                return CultureInfo.GetCultureInfo(culture.Trim());
            }
            catch
            {
                try
                {
                    return CultureInfo.GetCultureInfo(culture.Trim().Split('-')[0]);
                }
                catch
                {
                    return CultureInfo.GetCultureInfo("en-US");
                }
            }
        }
    }
}
