import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoadingService } from './loading.service';
import { User, LoginCredentials, AuthResponse, SsoProviderId, UpdateProfileRequest } from '../models/user.model';

export interface TranslationResponse {
  culture: string;
  translations: Record<string, string>;
}

export interface ApiRequestOptions {
  headers?: HttpHeaders;
  params?: HttpParams;
  blockUi?: boolean;
}

const BASE_URL = 'http://localhost:5233/api';

export const API_ENDPOINTS = {
  AUTH_LOGIN: `${BASE_URL}/auth/login`,
  AUTH_SSO_LOGIN: `${BASE_URL}/auth/sso-login`,
  AUTH_ME: `${BASE_URL}/auth/me`,
  PROFILE: `${BASE_URL}/profile`,
  PROFILE_EMAIL: `${BASE_URL}/profile/email`,
  PROFILE_LANGUAGE: `${BASE_URL}/profile/language`,
  TRANSLATIONS_PUBLIC: `${BASE_URL}/translations/public`,
  TRANSLATIONS_AUTHENTICATED: `${BASE_URL}/translations/authenticated`
} as const;

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient, { optional: true });
  private loadingService = inject(LoadingService);

  readonly endpoints = API_ENDPOINTS;

  // ==========================================
  // AUTHENTICATION API METHODS
  // ==========================================

  /**
   * Authenticates user with email and password
   */
  login(credentials: LoginCredentials, options: ApiRequestOptions = {}): Observable<AuthResponse> {
    return this.post<AuthResponse>(API_ENDPOINTS.AUTH_LOGIN, credentials, options);
  }

  /**
   * Authenticates user via Single Sign-On (Google, Azure, GitHub)
   */
  loginWithSso(provider: SsoProviderId, rememberMe: boolean = true, options: ApiRequestOptions = {}): Observable<AuthResponse> {
    return this.post<AuthResponse>(API_ENDPOINTS.AUTH_SSO_LOGIN, { provider, rememberMe }, options);
  }

  /**
   * Retrieves current authenticated user details from /api/auth/me
   */
  getCurrentUser(options: ApiRequestOptions = {}): Observable<User> {
    return this.get<User>(API_ENDPOINTS.AUTH_ME, options);
  }

  // ==========================================
  // USER PROFILE API METHODS
  // ==========================================

  /**
   * Retrieves authenticated user's profile from /api/profile
   */
  getProfile(options: ApiRequestOptions = {}): Observable<User> {
    return this.get<User>(API_ENDPOINTS.PROFILE, options);
  }

  /**
   * Updates user's entire profile (firstName, lastName, preferredLanguage, avatarUrl)
   */
  updateProfile(profile: UpdateProfileRequest, options: ApiRequestOptions = {}): Observable<User> {
    return this.put<User>(API_ENDPOINTS.PROFILE, profile, options);
  }

  /**
   * Updates user email address with syntax and uniqueness validation
   */
  updateEmail(newEmail: string, options: ApiRequestOptions = {}): Observable<User> {
    return this.put<User>(API_ENDPOINTS.PROFILE_EMAIL, { newEmail }, options);
  }

  /**
   * Updates user's preferred language in database
   */
  updateLanguage(language: string, options: ApiRequestOptions = {}): Observable<User> {
    return this.put<User>(API_ENDPOINTS.PROFILE_LANGUAGE, { language }, options);
  }

  // ==========================================
  // TRANSLATION API METHODS
  // ==========================================

  /**
   * Fetches public translation dictionary for unauthenticated views
   */
  getPublicTranslations(culture: string, options: ApiRequestOptions = { blockUi: true }): Observable<TranslationResponse> {
    const params = (options.params ?? new HttpParams()).set('culture', culture);
    return this.get<TranslationResponse>(API_ENDPOINTS.TRANSLATIONS_PUBLIC, { ...options, params });
  }

  /**
   * Fetches authenticated translation dictionary for authenticated views
   */
  getAuthenticatedTranslations(culture: string, options: ApiRequestOptions = { blockUi: true }): Observable<TranslationResponse> {
    const params = (options.params ?? new HttpParams()).set('culture', culture);
    return this.get<TranslationResponse>(API_ENDPOINTS.TRANSLATIONS_AUTHENTICATED, { ...options, params });
  }

  // ==========================================
  // GENERIC HTTP HELPERS
  // ==========================================

  /**
   * Executes an HTTP GET request with optional UI blocking
   */
  get<T>(url: string, options: ApiRequestOptions = {}): Observable<T> {
    return this.executeRequest<T>(
      (opt) => this.http?.get<T>(url, opt) ?? new Observable<T>(),
      options
    );
  }

  /**
   * Executes an HTTP POST request with optional UI blocking
   */
  post<T>(url: string, body: any, options: ApiRequestOptions = {}): Observable<T> {
    return this.executeRequest<T>(
      (opt) => this.http?.post<T>(url, body, opt) ?? new Observable<T>(),
      options
    );
  }

  /**
   * Executes an HTTP PUT request with optional UI blocking
   */
  put<T>(url: string, body: any, options: ApiRequestOptions = {}): Observable<T> {
    return this.executeRequest<T>(
      (opt) => this.http?.put<T>(url, body, opt) ?? new Observable<T>(),
      options
    );
  }

  /**
   * Executes an HTTP DELETE request with optional UI blocking
   */
  delete<T>(url: string, options: ApiRequestOptions = {}): Observable<T> {
    return this.executeRequest<T>(
      (opt) => this.http?.delete<T>(url, opt) ?? new Observable<T>(),
      options
    );
  }

  /**
   * Internal wrapper that manages UI blocking via LoadingService
   */
  private executeRequest<T>(
    requestFn: (opt: { headers?: HttpHeaders; params?: HttpParams }) => Observable<T>,
    options: ApiRequestOptions
  ): Observable<T> {
    const shouldBlock = options.blockUi ?? true;

    if (shouldBlock) {
      this.loadingService.blockUi();
    }

    const httpOptions = {
      headers: options.headers,
      params: options.params
    };

    return requestFn(httpOptions).pipe(
      finalize(() => {
        if (shouldBlock) {
          this.loadingService.unblockUi();
        }
      })
    );
  }
}
