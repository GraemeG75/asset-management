import { Injectable, signal, computed, inject, effect } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, of, tap } from 'rxjs';
import { UserService } from './user.service';
import { ApiService } from './api.service';

export interface TranslationResponse {
  culture: string;
  translations: Record<string, string>;
}

export interface LanguageOption {
  code: string;
  name: string;
  flag: string;
}

const API_BASE_URL = 'http://localhost:5000/api/translations';

export const AVAILABLE_LANGUAGES: LanguageOption[] = [
  { code: 'en', name: 'English', flag: '🇺🇸' },
  { code: 'es', name: 'Español', flag: '🇪🇸' },
  { code: 'fr', name: 'Français', flag: '🇫🇷' },
  { code: 'de', name: 'Deutsch', flag: '🇩🇪' }
];

const MULTI_PUBLIC_DICTIONARIES: Record<string, Record<string, string>> = {
  en: {
    'APP_TITLE': 'AssetPulse',
    'LOGIN_TITLE': 'Sign in to AssetPulse',
    'LOGIN_SUBTITLE': 'Manage enterprise assets, track inventory, and access platform features.',
    'EMAIL_LABEL': 'Email Address',
    'EMAIL_PLACEHOLDER': 'e.g. admin@assetmgmt.io',
    'EMAIL_REQUIRED': 'Email is required',
    'EMAIL_INVALID': 'Please enter a valid email address',
    'PASSWORD_LABEL': 'Password',
    'PASSWORD_PLACEHOLDER': '••••••••',
    'PASSWORD_REQUIRED': 'Password is required',
    'PASSWORD_MINLENGTH': 'Password must be at least 4 characters',
    'REMEMBER_ME': 'Remember me on this device',
    'SIGN_IN_BTN': 'Sign In',
    'AUTHENTICATING': 'Authenticating...',
    'OR_SIGN_IN_WITH': 'OR SIGN IN WITH',
    'SSO_GOOGLE': 'Google',
    'SSO_MICROSOFT': 'Microsoft',
    'SSO_GITHUB': 'GitHub',
    'NAV_BRAND': 'AssetPulse Platform',
    'DEMO_CREDENTIALS': 'Demo Credentials: admin@assetmgmt.io / password123',
    'LANGUAGE_SELECTOR': 'Language'
  },
  es: {
    'APP_TITLE': 'AssetPulse',
    'LOGIN_TITLE': 'Iniciar sesión en AssetPulse',
    'LOGIN_SUBTITLE': 'Gestione activos empresariales, rastree inventarios y acceda a las funciones.',
    'EMAIL_LABEL': 'Correo electrónico',
    'EMAIL_PLACEHOLDER': 'ej. admin@assetmgmt.io',
    'EMAIL_REQUIRED': 'El correo electrónico es obligatorio',
    'EMAIL_INVALID': 'Ingrese un correo electrónico válido',
    'PASSWORD_LABEL': 'Contraseña',
    'PASSWORD_PLACEHOLDER': '••••••••',
    'PASSWORD_REQUIRED': 'La contraseña es obligatoria',
    'PASSWORD_MINLENGTH': 'La contraseña debe tener al menos 4 caracteres',
    'REMEMBER_ME': 'Recordarme en este dispositivo',
    'SIGN_IN_BTN': 'Iniciar sesión',
    'AUTHENTICATING': 'Autenticando...',
    'OR_SIGN_IN_WITH': 'O INICIAR SESIÓN CON',
    'SSO_GOOGLE': 'Google',
    'SSO_MICROSOFT': 'Microsoft',
    'SSO_GITHUB': 'GitHub',
    'NAV_BRAND': 'Plataforma AssetPulse',
    'DEMO_CREDENTIALS': 'Credenciales demo: admin@assetmgmt.io / password123',
    'LANGUAGE_SELECTOR': 'Idioma'
  },
  fr: {
    'APP_TITLE': 'AssetPulse',
    'LOGIN_TITLE': 'Connexion à AssetPulse',
    'LOGIN_SUBTITLE': "Gérez les actifs de l'entreprise, suivez l'inventaire et accédez aux fonctionnalités.",
    'EMAIL_LABEL': 'Adresse e-mail',
    'EMAIL_PLACEHOLDER': 'ex. admin@assetmgmt.io',
    'EMAIL_REQUIRED': "L'e-mail est requis",
    'EMAIL_INVALID': 'Veuillez entrer une adresse e-mail valide',
    'PASSWORD_LABEL': 'Mot de passe',
    'PASSWORD_PLACEHOLDER': '••••••••',
    'PASSWORD_REQUIRED': 'Le mot de passe est requis',
    'PASSWORD_MINLENGTH': 'Le mot de passe doit contenir au moins 4 caractères',
    'REMEMBER_ME': 'Se souvenir de moi sur cet appareil',
    'SIGN_IN_BTN': 'Se connecter',
    'AUTHENTICATING': 'Authentification...',
    'OR_SIGN_IN_WITH': 'OU SE CONNECTER AVEC',
    'SSO_GOOGLE': 'Google',
    'SSO_MICROSOFT': 'Microsoft',
    'SSO_GITHUB': 'GitHub',
    'NAV_BRAND': 'Plateforme AssetPulse',
    'DEMO_CREDENTIALS': 'Identifiants démo : admin@assetmgmt.io / password123',
    'LANGUAGE_SELECTOR': 'Langue'
  },
  de: {
    'APP_TITLE': 'AssetPulse',
    'LOGIN_TITLE': 'Anmelden bei AssetPulse',
    'LOGIN_SUBTITLE': 'Verwalten Sie Unternehmenswerte, verfolgen Sie Bestände und greifen Sie auf Funktionen zu.',
    'EMAIL_LABEL': 'E-Mail-Adresse',
    'EMAIL_PLACEHOLDER': 'z.B. admin@assetmgmt.io',
    'EMAIL_REQUIRED': 'E-Mail-Adresse ist erforderlich',
    'EMAIL_INVALID': 'Bitte geben Sie eine gültige E-Mail-Adresse ein',
    'PASSWORD_LABEL': 'Passwort',
    'PASSWORD_PLACEHOLDER': '••••••••',
    'PASSWORD_REQUIRED': 'Passwort ist erforderlich',
    'PASSWORD_MINLENGTH': 'Passwort muss mindestens 4 Zeichen lang sein',
    'REMEMBER_ME': 'Auf diesem Gerät angemeldet bleiben',
    'SIGN_IN_BTN': 'Anmelden',
    'AUTHENTICATING': 'Authentifizierung...',
    'OR_SIGN_IN_WITH': 'ODER ANMELDEN MIT',
    'SSO_GOOGLE': 'Google',
    'SSO_MICROSOFT': 'Microsoft',
    'SSO_GITHUB': 'GitHub',
    'NAV_BRAND': 'AssetPulse Plattform',
    'DEMO_CREDENTIALS': 'Demo-Zugangsdaten: admin@assetmgmt.io / password123',
    'LANGUAGE_SELECTOR': 'Sprache'
  }
};

const MULTI_AUTH_DICTIONARIES: Record<string, Record<string, string>> = {
  en: {
    'NAV_DASHBOARD': 'Dashboard',
    'NAV_ASSETS': 'Asset Inventory',
    'NAV_CATEGORIES': 'Categories',
    'NAV_REPORTS': 'Reports',
    'NAV_SETTINGS': 'Settings',
    'NAV_LOGOUT': 'Sign Out',
    'USER_PROFILE': 'User Profile',
    'ROLE_ADMIN': 'Administrator',
    'ROLE_MANAGER': 'Asset Manager',
    'ROLE_USER': 'Standard User',
    'WELCOME_BACK': 'Welcome back',
    'TOTAL_ASSETS': 'Total Assets',
    'ACTIVE_ASSETS': 'Active Assets',
    'MAINTENANCE_DUE': 'Maintenance Due',
    'SYSTEM_HEALTH': 'System Status'
  },
  es: {
    'NAV_DASHBOARD': 'Panel principal',
    'NAV_ASSETS': 'Inventario de activos',
    'NAV_CATEGORIES': 'Categorías',
    'NAV_REPORTS': 'Informes',
    'NAV_SETTINGS': 'Configuración',
    'NAV_LOGOUT': 'Cerrar sesión',
    'USER_PROFILE': 'Perfil de usuario',
    'ROLE_ADMIN': 'Administrador',
    'ROLE_MANAGER': 'Gerente de activos',
    'ROLE_USER': 'Usuario estándar',
    'WELCOME_BACK': 'Bienvenido de nuevo',
    'TOTAL_ASSETS': 'Activos totales',
    'ACTIVE_ASSETS': 'Activos activos',
    'MAINTENANCE_DUE': 'Mantenimiento pendiente',
    'SYSTEM_HEALTH': 'Estado del sistema'
  },
  fr: {
    'NAV_DASHBOARD': 'Tableau de bord',
    'NAV_ASSETS': 'Inventaire des actifs',
    'NAV_CATEGORIES': 'Catégories',
    'NAV_REPORTS': 'Rapports',
    'NAV_SETTINGS': 'Paramètres',
    'NAV_LOGOUT': 'Se déconnecter',
    'USER_PROFILE': 'Profil utilisateur',
    'ROLE_ADMIN': 'Administrateur',
    'ROLE_MANAGER': "Gestionnaire d'actifs",
    'ROLE_USER': 'Utilisateur standard',
    'WELCOME_BACK': 'Bon retour',
    'TOTAL_ASSETS': 'Total des actifs',
    'ACTIVE_ASSETS': 'Actifs actifs',
    'MAINTENANCE_DUE': 'Maintenance due',
    'SYSTEM_HEALTH': 'État du système'
  },
  de: {
    'NAV_DASHBOARD': 'Dashboard',
    'NAV_ASSETS': 'Anlageninventar',
    'NAV_CATEGORIES': 'Kategorien',
    'NAV_REPORTS': 'Berichte',
    'NAV_SETTINGS': 'Einstellungen',
    'NAV_LOGOUT': 'Abmelden',
    'USER_PROFILE': 'Benutzerprofil',
    'ROLE_ADMIN': 'Administrator',
    'ROLE_MANAGER': 'Asset-Manager',
    'ROLE_USER': 'Standardbenutzer',
    'WELCOME_BACK': 'Willkommen zurück',
    'TOTAL_ASSETS': 'Gesamte Anlagen',
    'ACTIVE_ASSETS': 'Aktive Anlagen',
    'MAINTENANCE_DUE': 'Wartung fällig',
    'SYSTEM_HEALTH': 'Systemstatus'
  }
};

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private apiService = inject(ApiService, { optional: true });
  private http = inject(HttpClient, { optional: true });
  private userService = inject(UserService);

  readonly currentCulture = signal<string>('en');
  readonly translations = signal<Record<string, string>>(MULTI_PUBLIC_DICTIONARIES['en']);
  readonly availableLanguages = AVAILABLE_LANGUAGES;

  constructor() {
    this.loadPublicTranslations();

    // Automatically synchronize language with authenticated user profile or initial login
    effect(() => {
      const user = typeof this.userService.currentUser === 'function' ? this.userService.currentUser() : null;
      const isLoggedIn = typeof this.userService.isLoggedIn === 'function' ? this.userService.isLoggedIn() : false;

      if (user && user.preferredLanguage && user.preferredLanguage !== this.currentCulture()) {
        this.setCulture(user.preferredLanguage);
      } else if (isLoggedIn) {
        this.loadAuthenticatedTranslations(this.currentCulture());
      } else {
        this.loadPublicTranslations(this.currentCulture());
      }
    });
  }

  /**
   * Sets active culture and reloads appropriate translation dictionary
   */
  setCulture(culture: string): void {
    if (!culture) return;
    const normalizedCulture = culture.toLowerCase();
    this.currentCulture.set(normalizedCulture);

    if (this.userService.isLoggedIn()) {
      // Save language to user profile DB via backend API
      this.userService.updatePreferredLanguage(normalizedCulture).subscribe();
      this.loadAuthenticatedTranslations(normalizedCulture);
    } else {
      this.loadPublicTranslations(normalizedCulture);
    }
  }

  /**
   * Fetches public translation dictionary from /api/translations/public via ApiService
   */
  loadPublicTranslations(culture: string = this.currentCulture()): void {
    const fallback = MULTI_PUBLIC_DICTIONARIES[culture] || MULTI_PUBLIC_DICTIONARIES['en'];
    const url = `${API_BASE_URL}/public?culture=${culture}`;
    const request$ = this.apiService 
      ? this.apiService.get<TranslationResponse>(url, { blockUi: false })
      : (this.http ? this.http.get<TranslationResponse>(url) : null);

    if (request$) {
      request$.pipe(
        tap(res => {
          this.translations.set({ ...fallback, ...res.translations });
          this.currentCulture.set(res.culture);
        }),
        catchError(() => {
          this.translations.set(fallback);
          return of(null);
        })
      ).subscribe();
    } else {
      this.translations.set(fallback);
    }
  }

  /**
   * Fetches authenticated translation dictionary from /api/translations/authenticated via ApiService
   */
  loadAuthenticatedTranslations(culture: string = this.currentCulture()): void {
    const token = this.userService.jwtToken();
    const fallbackAuth = MULTI_AUTH_DICTIONARIES[culture] || MULTI_AUTH_DICTIONARIES['en'];
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const url = `${API_BASE_URL}/authenticated?culture=${culture}`;
    const request$ = this.apiService
      ? this.apiService.get<TranslationResponse>(url, { headers, blockUi: false })
      : (this.http ? this.http.get<TranslationResponse>(url, { headers }) : null);

    if (request$) {
      request$.pipe(
        tap(res => {
          this.translations.update(current => ({
            ...current,
            ...fallbackAuth,
            ...res.translations
          }));
          this.currentCulture.set(res.culture);
        }),
        catchError(() => {
          this.translations.update(current => ({ ...current, ...fallbackAuth }));
          return of(null);
        })
      ).subscribe();
    } else {
      this.translations.update(current => ({ ...current, ...fallbackAuth }));
    }
  }

  /**
   * Translates a key using the active dictionary with optional fallback string
   */
  translate(key: string, defaultText?: string): string {
    const dictionary = this.translations();
    return dictionary[key] ?? defaultText ?? key;
  }
}
