import { Injectable, signal, inject, effect, untracked } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, of, tap, finalize } from 'rxjs';
import { UserService } from './user.service';
import { ApiService, TranslationResponse } from './api.service';

export interface LanguageOption {
  code: string;
  name: string;
  flag: string;
}

export const AVAILABLE_LANGUAGES: LanguageOption[] = [
  { code: 'en', name: 'English', flag: '🇺🇸' },
  { code: 'es', name: 'Español', flag: '🇪🇸' },
  { code: 'fr', name: 'Français', flag: '🇫🇷' },
  { code: 'de', name: 'Deutsch', flag: '🇩🇪' }
];

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private apiService = inject(ApiService, { optional: true });
  private http = inject(HttpClient, { optional: true });
  private userService = inject(UserService);

  readonly currentCulture = signal<string>('en');
  readonly translations = signal<Record<string, string>>({});
  readonly availableLanguages = AVAILABLE_LANGUAGES;

  private activeFetchKey: string | null = null;

  constructor() {
    // Automatically synchronize language with authenticated user profile or initial login
    effect(() => {
      const user = typeof this.userService.currentUser === 'function' ? this.userService.currentUser() : null;
      const isLoggedIn = typeof this.userService.isLoggedIn === 'function' ? this.userService.isLoggedIn() : false;
      const activeCulture = untracked(() => this.currentCulture());

      if (user && user.preferredLanguage && user.preferredLanguage !== activeCulture) {
        this.setCulture(user.preferredLanguage);
      } else if (isLoggedIn) {
        this.loadAuthenticatedTranslations(activeCulture);
      } else {
        this.loadPublicTranslations(activeCulture);
      }
    });
  }

  /**
   * Sets active culture and reloads appropriate translation dictionary from DB
   */
  setCulture(culture: string): void {
    if (!culture) return;
    const normalizedCulture = culture.toLowerCase();
    this.currentCulture.set(normalizedCulture);

    if (this.userService.isLoggedIn()) {
      this.userService.updatePreferredLanguage(normalizedCulture).subscribe();
      this.loadAuthenticatedTranslations(normalizedCulture);
    } else {
      this.loadPublicTranslations(normalizedCulture);
    }
  }

  /**
   * Fetches public translation dictionary from backend API exclusively from DB
   */
  loadPublicTranslations(culture: string = this.currentCulture()): void {
    const normalizedCulture = culture.toLowerCase();
    const fetchKey = `public_${normalizedCulture}`;
    if (this.activeFetchKey === fetchKey) {
      return;
    }

    this.activeFetchKey = fetchKey;
    const request$ = this.apiService?.getPublicTranslations(normalizedCulture, { blockUi: true });

    if (request$) {
      request$.pipe(
        tap(res => {
          this.translations.set(res.translations ?? {});
          this.currentCulture.set(res.culture);
        }),
        catchError(() => {
          return of(null);
        }),
        finalize(() => {
          this.activeFetchKey = null;
        })
      ).subscribe();
    } else {
      this.activeFetchKey = null;
    }
  }

  /**
   * Fetches authenticated translation dictionary from backend API exclusively from DB
   */
  loadAuthenticatedTranslations(culture: string = this.currentCulture()): void {
    const normalizedCulture = culture.toLowerCase();
    const fetchKey = `auth_${normalizedCulture}`;
    if (this.activeFetchKey === fetchKey) {
      return;
    }

    const token = this.userService.jwtToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    this.activeFetchKey = fetchKey;
    const request$ = this.apiService?.getAuthenticatedTranslations(normalizedCulture, { headers, blockUi: true });

    if (request$) {
      request$.pipe(
        tap(res => {
          this.translations.set(res.translations ?? {});
          this.currentCulture.set(res.culture);
        }),
        catchError(() => {
          return of(null);
        }),
        finalize(() => {
          this.activeFetchKey = null;
        })
      ).subscribe();
    } else {
      this.activeFetchKey = null;
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
