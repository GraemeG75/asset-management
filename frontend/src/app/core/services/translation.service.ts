import { Injectable, signal, computed, inject, effect } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, of, tap } from 'rxjs';
import { UserService } from './user.service';

export interface TranslationResponse {
  culture: string;
  translations: Record<string, string>;
}

const API_BASE_URL = 'http://localhost:5000/api/translations';

const DEFAULT_PUBLIC_DICTIONARY: Record<string, string> = {
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
  'DEMO_CREDENTIALS': 'Demo Credentials: admin@assetmgmt.io / password123'
};

const DEFAULT_AUTH_DICTIONARY: Record<string, string> = {
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
};

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private http = inject(HttpClient, { optional: true });
  private userService = inject(UserService);

  readonly currentCulture = signal<string>('en');
  readonly translations = signal<Record<string, string>>(DEFAULT_PUBLIC_DICTIONARY);

  constructor() {
    this.loadPublicTranslations();

    // Automatically load authenticated translations when user logs in
    effect(() => {
      if (this.userService.isLoggedIn()) {
        this.loadAuthenticatedTranslations();
      } else {
        this.loadPublicTranslations();
      }
    });
  }

  /**
   * Fetches public translation dictionary from /api/translations/public
   */
  loadPublicTranslations(culture: string = this.currentCulture()): void {
    if (this.http) {
      this.http.get<TranslationResponse>(`${API_BASE_URL}/public?culture=${culture}`).pipe(
        tap(res => {
          this.translations.set({ ...DEFAULT_PUBLIC_DICTIONARY, ...res.translations });
          this.currentCulture.set(res.culture);
        }),
        catchError(() => {
          this.translations.set(DEFAULT_PUBLIC_DICTIONARY);
          return of(null);
        })
      ).subscribe();
    } else {
      this.translations.set(DEFAULT_PUBLIC_DICTIONARY);
    }
  }

  /**
   * Fetches authenticated translation dictionary from /api/translations/authenticated
   */
  loadAuthenticatedTranslations(culture: string = this.currentCulture()): void {
    const token = this.userService.jwtToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    if (this.http) {
      this.http.get<TranslationResponse>(`${API_BASE_URL}/authenticated?culture=${culture}`, { headers }).pipe(
        tap(res => {
          this.translations.update(current => ({
            ...current,
            ...DEFAULT_AUTH_DICTIONARY,
            ...res.translations
          }));
          this.currentCulture.set(res.culture);
        }),
        catchError(() => {
          this.translations.update(current => ({ ...current, ...DEFAULT_AUTH_DICTIONARY }));
          return of(null);
        })
      ).subscribe();
    } else {
      this.translations.update(current => ({ ...current, ...DEFAULT_AUTH_DICTIONARY }));
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
