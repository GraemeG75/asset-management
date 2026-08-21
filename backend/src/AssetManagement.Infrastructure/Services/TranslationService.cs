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

            Dictionary<string, Dictionary<string, string>> publicTranslations = new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string>
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
                    ["DEMO_CREDENTIALS"] = "Demo Credentials: admin@assetmgmt.io / password123",
                    ["LANGUAGE_SELECTOR"] = "Language"
                },
                ["es"] = new Dictionary<string, string>
                {
                    ["APP_TITLE"] = "AssetPulse",
                    ["LOGIN_TITLE"] = "Iniciar sesión en AssetPulse",
                    ["LOGIN_SUBTITLE"] = "Gestione activos empresariales, rastree inventarios y acceda a las funciones.",
                    ["EMAIL_LABEL"] = "Correo electrónico",
                    ["EMAIL_PLACEHOLDER"] = "ej. admin@assetmgmt.io",
                    ["EMAIL_REQUIRED"] = "El correo electrónico es obligatorio",
                    ["EMAIL_INVALID"] = "Ingrese un correo electrónico válido",
                    ["PASSWORD_LABEL"] = "Contraseña",
                    ["PASSWORD_PLACEHOLDER"] = "••••••••",
                    ["PASSWORD_REQUIRED"] = "La contraseña es obligatoria",
                    ["PASSWORD_MINLENGTH"] = "La contraseña debe tener al menos 4 caracteres",
                    ["REMEMBER_ME"] = "Recordarme en este dispositivo",
                    ["SIGN_IN_BTN"] = "Iniciar sesión",
                    ["AUTHENTICATING"] = "Autenticando...",
                    ["OR_SIGN_IN_WITH"] = "O INICIAR SESIÓN CON",
                    ["SSO_GOOGLE"] = "Google",
                    ["SSO_MICROSOFT"] = "Microsoft",
                    ["SSO_GITHUB"] = "GitHub",
                    ["NAV_BRAND"] = "Plataforma AssetPulse",
                    ["DEMO_CREDENTIALS"] = "Credenciales demo: admin@assetmgmt.io / password123",
                    ["LANGUAGE_SELECTOR"] = "Idioma"
                },
                ["fr"] = new Dictionary<string, string>
                {
                    ["APP_TITLE"] = "AssetPulse",
                    ["LOGIN_TITLE"] = "Connexion à AssetPulse",
                    ["LOGIN_SUBTITLE"] = "Gérez les actifs de l'entreprise, suivez l'inventaire et accédez aux fonctionnalités.",
                    ["EMAIL_LABEL"] = "Adresse e-mail",
                    ["EMAIL_PLACEHOLDER"] = "ex. admin@assetmgmt.io",
                    ["EMAIL_REQUIRED"] = "L'e-mail est requis",
                    ["EMAIL_INVALID"] = "Veuillez entrer une adresse e-mail valide",
                    ["PASSWORD_LABEL"] = "Mot de passe",
                    ["PASSWORD_PLACEHOLDER"] = "••••••••",
                    ["PASSWORD_REQUIRED"] = "Le mot de passe est requis",
                    ["PASSWORD_MINLENGTH"] = "Le mot de passe doit contenir au moins 4 caractères",
                    ["REMEMBER_ME"] = "Se souvenir de moi sur cet appareil",
                    ["SIGN_IN_BTN"] = "Se connecter",
                    ["AUTHENTICATING"] = "Authentification...",
                    ["OR_SIGN_IN_WITH"] = "OU SE CONNECTER AVEC",
                    ["SSO_GOOGLE"] = "Google",
                    ["SSO_MICROSOFT"] = "Microsoft",
                    ["SSO_GITHUB"] = "GitHub",
                    ["NAV_BRAND"] = "Plateforme AssetPulse",
                    ["DEMO_CREDENTIALS"] = "Identifiants démo : admin@assetmgmt.io / password123",
                    ["LANGUAGE_SELECTOR"] = "Langue"
                },
                ["de"] = new Dictionary<string, string>
                {
                    ["APP_TITLE"] = "AssetPulse",
                    ["LOGIN_TITLE"] = "Anmelden bei AssetPulse",
                    ["LOGIN_SUBTITLE"] = "Verwalten Sie Unternehmenswerte, verfolgen Sie Bestände und greifen Sie auf Funktionen zu.",
                    ["EMAIL_LABEL"] = "E-Mail-Adresse",
                    ["EMAIL_PLACEHOLDER"] = "z.B. admin@assetmgmt.io",
                    ["EMAIL_REQUIRED"] = "E-Mail-Adresse ist erforderlich",
                    ["EMAIL_INVALID"] = "Bitte geben Sie eine gültige E-Mail-Adresse ein",
                    ["PASSWORD_LABEL"] = "Passwort",
                    ["PASSWORD_PLACEHOLDER"] = "••••••••",
                    ["PASSWORD_REQUIRED"] = "Passwort ist erforderlich",
                    ["PASSWORD_MINLENGTH"] = "Passwort muss mindestens 4 Zeichen lang sein",
                    ["REMEMBER_ME"] = "Auf diesem Gerät angemeldet bleiben",
                    ["SIGN_IN_BTN"] = "Anmelden",
                    ["AUTHENTICATING"] = "Authentifizierung...",
                    ["OR_SIGN_IN_WITH"] = "ODER ANMELDEN MIT",
                    ["SSO_GOOGLE"] = "Google",
                    ["SSO_MICROSOFT"] = "Microsoft",
                    ["SSO_GITHUB"] = "GitHub",
                    ["NAV_BRAND"] = "AssetPulse Plattform",
                    ["DEMO_CREDENTIALS"] = "Demo-Zugangsdaten: admin@assetmgmt.io / password123",
                    ["LANGUAGE_SELECTOR"] = "Sprache"
                }
            };

            Dictionary<string, string> dictionary = publicTranslations.GetValueOrDefault(selectedCulture, publicTranslations["en"]);
            return new TranslationResponseDto(selectedCulture, dictionary);
        }

        public TranslationResponseDto GetAuthenticatedTranslations(string? culture = "en")
        {
            string selectedCulture = string.IsNullOrWhiteSpace(culture) ? "en" : culture.ToLower();

            Dictionary<string, Dictionary<string, string>> authTranslations = new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string>
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
                },
                ["es"] = new Dictionary<string, string>
                {
                    ["NAV_DASHBOARD"] = "Panel principal",
                    ["NAV_ASSETS"] = "Inventario de activos",
                    ["NAV_CATEGORIES"] = "Categorías",
                    ["NAV_REPORTS"] = "Informes",
                    ["NAV_SETTINGS"] = "Configuración",
                    ["NAV_LOGOUT"] = "Cerrar sesión",
                    ["USER_PROFILE"] = "Perfil de usuario",
                    ["ROLE_ADMIN"] = "Administrador",
                    ["ROLE_MANAGER"] = "Gerente de activos",
                    ["ROLE_USER"] = "Usuario estándar",
                    ["WELCOME_BACK"] = "Bienvenido de nuevo",
                    ["TOTAL_ASSETS"] = "Activos totales",
                    ["ACTIVE_ASSETS"] = "Activos activos",
                    ["MAINTENANCE_DUE"] = "Mantenimiento pendiente",
                    ["SYSTEM_HEALTH"] = "Estado del sistema"
                },
                ["fr"] = new Dictionary<string, string>
                {
                    ["NAV_DASHBOARD"] = "Tableau de bord",
                    ["NAV_ASSETS"] = "Inventaire des actifs",
                    ["NAV_CATEGORIES"] = "Catégories",
                    ["NAV_REPORTS"] = "Rapports",
                    ["NAV_SETTINGS"] = "Paramètres",
                    ["NAV_LOGOUT"] = "Se déconnecter",
                    ["USER_PROFILE"] = "Profil utilisateur",
                    ["ROLE_ADMIN"] = "Administrateur",
                    ["ROLE_MANAGER"] = "Gestionnaire d'actifs",
                    ["ROLE_USER"] = "Utilisateur standard",
                    ["WELCOME_BACK"] = "Bon retour",
                    ["TOTAL_ASSETS"] = "Total des actifs",
                    ["ACTIVE_ASSETS"] = "Actifs actifs",
                    ["MAINTENANCE_DUE"] = "Maintenance due",
                    ["SYSTEM_HEALTH"] = "État du système"
                },
                ["de"] = new Dictionary<string, string>
                {
                    ["NAV_DASHBOARD"] = "Dashboard",
                    ["NAV_ASSETS"] = "Anlageninventar",
                    ["NAV_CATEGORIES"] = "Kategorien",
                    ["NAV_REPORTS"] = "Berichte",
                    ["NAV_SETTINGS"] = "Einstellungen",
                    ["NAV_LOGOUT"] = "Abmelden",
                    ["USER_PROFILE"] = "Benutzerprofil",
                    ["ROLE_ADMIN"] = "Administrator",
                    ["ROLE_MANAGER"] = "Asset-Manager",
                    ["ROLE_USER"] = "Standardbenutzer",
                    ["WELCOME_BACK"] = "Willkommen zurück",
                    ["TOTAL_ASSETS"] = "Gesamte Anlagen",
                    ["ACTIVE_ASSETS"] = "Aktive Anlagen",
                    ["MAINTENANCE_DUE"] = "Wartung fällig",
                    ["SYSTEM_HEALTH"] = "Systemstatus"
                }
            };

            Dictionary<string, string> dictionary = authTranslations.GetValueOrDefault(selectedCulture, authTranslations["en"]);
            return new TranslationResponseDto(selectedCulture, dictionary);
        }
    }
}
