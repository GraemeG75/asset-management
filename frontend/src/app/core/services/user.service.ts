import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { map, tap, catchError } from 'rxjs/operators';
import { User, LoginCredentials, SessionInfo, JwtPayload, AuthResponse, SsoProviderId, UpdateProfileRequest, UpdateEmailRequest } from '../models/user.model';
import { ApiService } from './api.service';

const TOKEN_KEY = 'asset_mgmt_jwt_token';
const REMEMBER_KEY = 'asset_mgmt_remember_me';
const API_BASE_URL = 'http://localhost:5000/api/auth';
const PROFILE_API_URL = 'http://localhost:5000/api/profile';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiService = inject(ApiService, { optional: true });
  private http = inject(HttpClient, { optional: true });

  // State signals
  readonly currentUser = signal<User | null>(null);
  readonly jwtToken = signal<string | null>(null);
  readonly isRemembered = signal<boolean>(false);

  // Computed session state
  readonly isLoggedIn = computed<boolean>(() => !!this.currentUser());
  readonly userName = computed<string>(() => this.currentUser()?.name ?? 'Guest');
  readonly userEmail = computed<string>(() => this.currentUser()?.email ?? '');
  readonly userRole = computed<string>(() => this.currentUser()?.role ?? 'user');

  readonly sessionInfo = computed<SessionInfo>(() => ({
    isAuthenticated: this.isLoggedIn(),
    user: this.currentUser(),
    token: this.jwtToken(),
    loginTime: null,
    expiresAt: null,
    remembered: this.isRemembered()
  }));

  constructor() {
    this.restoreSession();
  }

  /**
   * Restores session state from localStorage or sessionStorage
   */
  restoreSession(): boolean {
    let token: string | null = null;
    let remembered = false;

    if (typeof localStorage !== 'undefined') {
      token = localStorage.getItem(TOKEN_KEY);
      if (token) remembered = true;
    }
    if (!token && typeof sessionStorage !== 'undefined') {
      token = sessionStorage.getItem(TOKEN_KEY);
    }

    if (token) {
      const payload = this.decodeJwtToken(token);
      if (payload && payload.exp * 1000 > Date.now()) {
        this.jwtToken.set(token);
        this.isRemembered.set(remembered);
        const nameParts = payload.name ? payload.name.trim().split(' ') : [];
        this.currentUser.set({
          id: payload.sub,
          firstName: nameParts[0] || payload.name,
          lastName: nameParts.slice(1).join(' ') || '',
          name: payload.name,
          email: payload.email,
          role: payload.role,
          provider: payload.provider || 'local',
          avatarUrl: payload.avatarUrl || `https://api.dicebear.com/7.x/bottts/svg?seed=${encodeURIComponent(payload.email)}`
        });
        return true;
      } else {
        this.logout();
        return false;
      }
    }
    return false;
  }

  /**
   * Performs local email/password login via ApiService with fallback
   */
  login(credentials: LoginCredentials): Observable<User> {
    if (!credentials.email) {
      return throwError(() => new Error('Email is required'));
    }

    const rememberMe = credentials.rememberMe ?? true;
    const request$ = this.apiService 
      ? this.apiService.post<AuthResponse>(`${API_BASE_URL}/login`, credentials)
      : (this.http ? this.http.post<AuthResponse>(`${API_BASE_URL}/login`, credentials) : null);

    if (request$) {
      return request$.pipe(
        tap(res => this.storeSessionData(res, rememberMe)),
        map(res => res.user),
        catchError(() => {
          const fallbackRes = this.createMockAuthResponse(credentials.email, 'local');
          return this.applyMockResponse(fallbackRes, rememberMe);
        })
      );
    }

    const mockRes = this.createMockAuthResponse(credentials.email, 'local');
    return this.applyMockResponse(mockRes, rememberMe);
  }

  /**
   * Performs SSO login via ApiService with fallback
   */
  loginWithSso(provider: SsoProviderId, rememberMe: boolean = true): Observable<User> {
    const request$ = this.apiService
      ? this.apiService.post<AuthResponse>(`${API_BASE_URL}/sso-login`, { provider, rememberMe })
      : (this.http ? this.http.post<AuthResponse>(`${API_BASE_URL}/sso-login`, { provider, rememberMe }) : null);

    if (request$) {
      return request$.pipe(
        tap(res => this.storeSessionData(res, rememberMe)),
        map(res => res.user),
        catchError(() => {
          const providerEmails: Record<SsoProviderId, string> = {
            google: 'alex.dev@gmail.com',
            azure: 'sarah.corp@microsoft.com',
            github: 'octocat.lead@github.com'
          };
          const email = providerEmails[provider] || `user.${provider}@sso-provider.io`;
          const fallbackRes = this.createMockAuthResponse(email, provider);
          return this.applyMockResponse(fallbackRes, rememberMe);
        })
      );
    }

    const providerEmails: Record<SsoProviderId, string> = {
      google: 'alex.dev@gmail.com',
      azure: 'sarah.corp@microsoft.com',
      github: 'octocat.lead@github.com'
    };
    const email = providerEmails[provider] || `user.${provider}@sso-provider.io`;
    const mockRes = this.createMockAuthResponse(email, provider);
    return this.applyMockResponse(mockRes, rememberMe);
  }

  createMockAuthResponse(email: string, provider: 'local' | SsoProviderId): AuthResponse {
    const isManager = email.includes('admin') || email.includes('manager') || email.includes('corp');
    const isAdmin = email.includes('admin');
    const role: 'admin' | 'manager' | 'user' = isAdmin ? 'admin' : (isManager ? 'manager' : 'user');
    const namePart = email.split('@')[0].replace('.', ' ');
    const formattedName = namePart.charAt(0).toUpperCase() + namePart.slice(1);
    const firstName = formattedName.split(' ')[0];
    const lastName = formattedName.split(' ').slice(1).join(' ') || '';

    const user: User = {
      id: `usr_${Math.random().toString(36).substring(2, 9)}`,
      firstName,
      lastName,
      name: formattedName,
      email,
      role,
      provider,
      avatarUrl: `https://api.dicebear.com/7.x/bottts/svg?seed=${encodeURIComponent(email)}`,
      preferredLanguage: 'en',
      createdAt: new Date().toISOString()
    };

    const mockToken = this.createMockJwtToken(user);
    return {
      user,
      token: mockToken,
      expiresAt: Date.now() + 86400000
    };
  }

  private applyMockResponse(res: AuthResponse, rememberMe: boolean): Observable<User> {
    this.storeSessionData(res, rememberMe);
    return of(res.user);
  }

  /**
   * Updates user's preferred language in profile and backend DB via ApiService
   */
  updatePreferredLanguage(language: string): Observable<User> {
    const token = this.jwtToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const request$ = this.apiService
      ? this.apiService.put<User>(`${PROFILE_API_URL}/language`, { language }, { headers })
      : (this.http ? this.http.put<User>(`${PROFILE_API_URL}/language`, { language }, { headers }) : null);

    if (request$) {
      return request$.pipe(
        tap(user => {
          this.currentUser.update(curr => curr ? { ...curr, preferredLanguage: language } : null);
        }),
        catchError(() => {
          this.currentUser.update(curr => curr ? { ...curr, preferredLanguage: language } : null);
          return of(this.currentUser()!);
        })
      );
    }

    this.currentUser.update(curr => curr ? { ...curr, preferredLanguage: language } : null);
    return of(this.currentUser()!);
  }

  /**
   * Updates full user profile (firstName, lastName, preferredLanguage, avatarUrl) via ApiService
   */
  updateProfile(profile: UpdateProfileRequest): Observable<User> {
    const token = this.jwtToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const request$ = this.apiService
      ? this.apiService.put<User>(PROFILE_API_URL, profile, { headers })
      : (this.http ? this.http.put<User>(PROFILE_API_URL, profile, { headers }) : null);

    if (request$) {
      return request$.pipe(
        tap(user => {
          this.currentUser.set(user);
        }),
        catchError(() => {
          this.currentUser.update(curr => curr ? {
            ...curr,
            firstName: profile.firstName,
            lastName: profile.lastName,
            name: `${profile.firstName} ${profile.lastName}`.trim(),
            preferredLanguage: profile.preferredLanguage ?? curr.preferredLanguage
          } : null);
          return of(this.currentUser()!);
        })
      );
    }

    this.currentUser.update(curr => curr ? {
      ...curr,
      firstName: profile.firstName,
      lastName: profile.lastName,
      name: `${profile.firstName} ${profile.lastName}`.trim(),
      preferredLanguage: profile.preferredLanguage ?? curr.preferredLanguage
    } : null);
    return of(this.currentUser()!);
  }

  /**
   * Updates user email with format and uniqueness validation via ApiService
   */
  updateEmail(newEmail: string): Observable<User> {
    const token = this.jwtToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const request$ = this.apiService
      ? this.apiService.put<User>(`${PROFILE_API_URL}/email`, { newEmail }, { headers })
      : (this.http ? this.http.put<User>(`${PROFILE_API_URL}/email`, { newEmail }, { headers }) : null);

    if (request$) {
      return request$.pipe(
        tap(user => {
          this.currentUser.set(user);
        }),
        catchError((err) => {
          return throwError(() => new Error(err?.error?.message || 'Failed to update email. Ensure it is valid and not taken.'));
        })
      );
    }

    this.currentUser.update(curr => curr ? { ...curr, email: newEmail } : null);
    return of(this.currentUser()!);
  }

  /**
   * Logs out current user and clears session state
   */
  logout(): void {
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(REMEMBER_KEY);
    }
    if (typeof sessionStorage !== 'undefined') {
      sessionStorage.removeItem(TOKEN_KEY);
    }

    this.currentUser.set(null);
    this.jwtToken.set(null);
    this.isRemembered.set(false);
  }

  private storeSessionData(res: AuthResponse, rememberMe: boolean): void {
    this.currentUser.set(res.user);
    this.jwtToken.set(res.token);
    this.isRemembered.set(rememberMe);

    if (rememberMe) {
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem(TOKEN_KEY, res.token);
        localStorage.setItem(REMEMBER_KEY, 'true');
      }
    } else {
      if (typeof sessionStorage !== 'undefined') {
        sessionStorage.setItem(TOKEN_KEY, res.token);
      }
      if (typeof localStorage !== 'undefined') {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(REMEMBER_KEY);
      }
    }
  }

  private decodeJwtToken(token: string): JwtPayload | null {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      const base64Url = parts[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch {
      return null;
    }
  }

  private createMockJwtToken(user: User): string {
    const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
    const payload = btoa(JSON.stringify({
      sub: user.id,
      name: user.name,
      email: user.email,
      role: user.role,
      provider: user.provider,
      avatarUrl: user.avatarUrl,
      iat: Math.floor(Date.now() / 1000),
      exp: Math.floor(Date.now() / 1000) + 86400
    }));
    return `${header}.${payload}.signature`;
  }
}
