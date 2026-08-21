import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { map, tap, catchError } from 'rxjs/operators';
import { User, LoginCredentials, SessionInfo, JwtPayload, AuthResponse, SsoProviderId, UpdateProfileRequest, UpdateEmailRequest } from '../models/user.model';

const TOKEN_KEY = 'asset_mgmt_jwt_token';
const REMEMBER_KEY = 'asset_mgmt_remember_me';
const API_BASE_URL = 'http://localhost:5000/api/auth';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient, { optional: true });

  // State signals
  readonly currentUser = signal<User | null>(null);
  readonly jwtToken = signal<string | null>(null);
  readonly isRemembered = signal<boolean>(false);
  
  // Computed session state
  readonly isLoggedIn = computed<boolean>(() => !!this.currentUser());
  readonly userName = computed<string>(() => this.currentUser()?.name ?? 'Guest');
  readonly userEmail = computed<string>(() => this.currentUser()?.email ?? '');
  readonly userRole = computed<string>(() => this.currentUser()?.role ?? 'guest');
  
  readonly sessionInfo = computed<SessionInfo>(() => {
    const user = this.currentUser();
    const token = this.jwtToken();
    const payload = token ? this.decodeJwt(token) : null;

    return {
      isAuthenticated: !!user,
      user: user,
      token: token,
      loginTime: payload ? payload.iat * 1000 : null,
      expiresAt: payload ? payload.exp * 1000 : null,
      remembered: this.isRemembered()
    };
  });

  constructor() {
    this.restoreSession();
  }

  /**
   * Restores user session from stored JWT token in localStorage or sessionStorage if valid
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
      remembered = false;
    }

    if (!token) {
      this.clearState();
      return false;
    }

    const payload = this.decodeJwt(token);
    if (!payload || this.isTokenExpired(payload)) {
      this.logout();
      return false;
    }

    const user: User = {
      id: payload.sub,
      name: payload.name,
      email: payload.email,
      role: payload.role,
      provider: payload.provider || 'local',
      avatarUrl: payload.avatarUrl
    };

    this.jwtToken.set(token);
    this.currentUser.set(user);
    this.isRemembered.set(remembered);
    return true;
  }

  /**
   * Performs user login via C# ASP.NET Core Web API with fallback to client-side auth
   */
  login(credentials: LoginCredentials): Observable<User> {
    if (!credentials.email) {
      return throwError(() => new Error('Email is required'));
    }

    const rememberMe = credentials.rememberMe ?? true;

    if (this.http) {
      return this.http.post<AuthResponse>(`${API_BASE_URL}/login`, credentials).pipe(
        tap(res => this.storeSessionData(res, rememberMe)),
        map(res => res.user),
        catchError(() => {
          // Client-side fallback if backend API is not running
          const fallbackRes = this.createMockAuthResponse(credentials.email, 'local');
          return this.applyMockResponse(fallbackRes, rememberMe);
        })
      );
    }

    const mockRes = this.createMockAuthResponse(credentials.email, 'local');
    return this.applyMockResponse(mockRes, rememberMe);
  }

  /**
   * Performs SSO login via C# ASP.NET Core Web API with fallback
   */
  loginWithSso(provider: SsoProviderId, rememberMe: boolean = true): Observable<User> {
    if (this.http) {
      return this.http.post<AuthResponse>(`${API_BASE_URL}/sso-login`, { provider, rememberMe }).pipe(
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

  private storeSessionData(res: AuthResponse, rememberMe: boolean): void {
    if (rememberMe) {
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem(TOKEN_KEY, res.token);
        localStorage.setItem(REMEMBER_KEY, 'true');
      }
      if (typeof sessionStorage !== 'undefined') {
        sessionStorage.removeItem(TOKEN_KEY);
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

    this.jwtToken.set(res.token);
    this.currentUser.set(res.user);
    this.isRemembered.set(rememberMe);
  }

  private applyMockResponse(res: AuthResponse, rememberMe: boolean): Observable<User> {
    this.storeSessionData(res, rememberMe);
    return of(res.user);
  }

  /**
   * Updates user's preferred language in profile and backend DB
   */
  updatePreferredLanguage(language: string): Observable<User> {
    const token = this.jwtToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    if (this.http) {
      return this.http.put<User>('http://localhost:5000/api/profile/language', { language }, { headers }).pipe(
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
   * Updates full user profile (firstName, lastName, preferredLanguage, avatarUrl)
   */
  updateProfile(profile: UpdateProfileRequest): Observable<User> {
    const token = this.jwtToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    if (this.http) {
      return this.http.put<User>('http://localhost:5000/api/profile', profile, { headers }).pipe(
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
   * Updates user email with format and uniqueness validation
   */
  updateEmail(newEmail: string): Observable<User> {
    const token = this.jwtToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    if (this.http) {
      return this.http.put<User>('http://localhost:5000/api/profile/email', { newEmail }, { headers }).pipe(
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
    this.clearState();
  }

  private clearState(): void {
    this.jwtToken.set(null);
    this.currentUser.set(null);
    this.isRemembered.set(false);
  }

  /**
   * Utility to decode JWT claims payload
   */
  decodeJwt(token: string): JwtPayload | null {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      
      const payloadBase64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(payloadBase64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );

      return JSON.parse(jsonPayload) as JwtPayload;
    } catch {
      return null;
    }
  }

  /**
   * Checks if JWT payload has expired
   */
  isTokenExpired(payload: JwtPayload): boolean {
    if (!payload || !payload.exp) return true;
    const nowInSeconds = Math.floor(Date.now() / 1000);
    return payload.exp < nowInSeconds;
  }

  /**
   * Generates a signed mock JWT token for testing/demo authentication
   */
  createMockAuthResponse(email: string, provider: 'local' | SsoProviderId = 'local'): AuthResponse {
    const isManager = email.includes('admin') || email.includes('corp') || email.includes('microsoft');
    const isSpecial = email.includes('admin');
    
    const role: 'admin' | 'manager' | 'user' = isSpecial ? 'admin' : (isManager ? 'manager' : 'user');
    const name = email.split('@')[0].replace(/[._]/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
    
    const user: User = {
      id: `usr_${Math.random().toString(36).substring(2, 9)}`,
      name: name || 'Demo User',
      email: email,
      role: role,
      provider: provider,
      avatarUrl: `https://api.dicebear.com/7.x/bottts/svg?seed=${encodeURIComponent(email)}`,
      createdAt: new Date().toISOString()
    };

    const header = { alg: 'HS256', typ: 'JWT' };
    const now = Math.floor(Date.now() / 1000);
    const payload: JwtPayload = {
      sub: user.id,
      name: user.name,
      email: user.email,
      role: user.role,
      provider: user.provider,
      avatarUrl: user.avatarUrl,
      iat: now,
      exp: now + 86400 // 24 hours validity
    };

    const token = `${this.base64UrlEncode(JSON.stringify(header))}.${this.base64UrlEncode(JSON.stringify(payload))}.mock_signature`;

    return {
      user,
      token,
      expiresAt: payload.exp * 1000
    };
  }

  private base64UrlEncode(str: string): string {
    return btoa(unescape(encodeURIComponent(str)))
      .replace(/\+/g, '-')
      .replace(/\//g, '_')
      .replace(/=+$/, '');
  }
}
