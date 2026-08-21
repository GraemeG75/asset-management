using System.Collections.Generic;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Services;

namespace AssetManagement.Infrastructure.Services
{
    public class TranslationService : ITranslationService
    {
        public TranslationResponseDto GetPublicTranslations(string? culture = "en")
        {
            string selectedCulture = string.IsNullOrWhiteSpace(culture) ? "en" : culture.ToLower();

            Dictionary<string, string> publicDictionary = new Dictionary<string, string>
            {
                ["APP_TITLE"] = "AssetPulse",
                ["LOGIN_TITLE"] = "Sign in to AssetPulse",
                ["LOGIN_SUBTITLE"] = "Manage enterprise assets, track inventory, and access platform features.",
                ["EMAIL_LABEL"] = "Email Address",
                ["EMAIL_PLACEHOLDER"] = "e.g. admin@assetmgmt.io",
                ["EMAIL_REQUIRED"] = "Email is required",
                ["EMAIL_INVALID"] = "Please enter a valid email address",
                ["PASSWORD_LABEL"] = "Password",
                ["PASSWORD_PLACEHOLDER"] = "••••••••",
                ["PASSWORD_REQUIRED"] = "Password is required",
                ["PASSWORD_MINLENGTH"] = "Password must be at least 4 characters",
                ["REMEMBER_ME"] = "Remember me on this device",
                ["SIGN_IN_BTN"] = "Sign In",
                ["AUTHENTICATING"] = "Authenticating...",
                ["OR_SIGN_IN_WITH"] = "OR SIGN IN WITH",
                ["SSO_GOOGLE"] = "Google",
                ["SSO_MICROSOFT"] = "Microsoft",
                ["SSO_GITHUB"] = "GitHub",
                ["NAV_BRAND"] = "AssetPulse Platform",
                ["DEMO_CREDENTIALS"] = "Demo Credentials: admin@assetmgmt.io / password123"
            };

            return new TranslationResponseDto(selectedCulture, publicDictionary);
        }

        public TranslationResponseDto GetAuthenticatedTranslations(string? culture = "en")
        {
            string selectedCulture = string.IsNullOrWhiteSpace(culture) ? "en" : culture.ToLower();

            Dictionary<string, string> authDictionary = new Dictionary<string, string>
            {
                ["NAV_DASHBOARD"] = "Dashboard",
                ["NAV_ASSETS"] = "Asset Inventory",
                ["NAV_CATEGORIES"] = "Categories",
                ["NAV_REPORTS"] = "Reports",
                ["NAV_SETTINGS"] = "Settings",
                ["NAV_LOGOUT"] = "Sign Out",
                ["USER_PROFILE"] = "User Profile",
                ["ROLE_ADMIN"] = "Administrator",
                ["ROLE_MANAGER"] = "Asset Manager",
                ["ROLE_USER"] = "Standard User",
                ["WELCOME_BACK"] = "Welcome back",
                ["TOTAL_ASSETS"] = "Total Assets",
                ["ACTIVE_ASSETS"] = "Active Assets",
                ["MAINTENANCE_DUE"] = "Maintenance Due",
                ["SYSTEM_HEALTH"] = "System Status"
            };

            return new TranslationResponseDto(selectedCulture, authDictionary);
        }
    }
}
