import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { User, LoginCredentials, SessionInfo, JwtPayload, AuthResponse, SsoProviderId, UpdateProfileRequest } from '../models/user.model';
import { ApiService } from './api.service';

const TOKEN_KEY = 'asset_mgmt_jwt_token';
const REMEMBER_KEY = 'asset_mgmt_remember_me';

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
   * Performs local email/password login via ApiService connected to backend DB
   */
  login(credentials: LoginCredentials): Observable<User> {
    if (!credentials.email) {
      return throwError(() => new Error('Email is required'));
    }

    const rememberMe = credentials.rememberMe ?? true;
    if (!this.apiService) {
      return throwError(() => new Error('API service unavailable'));
    }

    return this.apiService.login(credentials).pipe(
      tap(res => this.storeSessionData(res, rememberMe)),
      map(res => res.user)
    );
  }

  /**
   * Performs SSO login via ApiService connected to backend DB
   */
  loginWithSso(provider: SsoProviderId, rememberMe: boolean = true): Observable<User> {
    if (!this.apiService) {
      return throwError(() => new Error('API service unavailable'));
    }

    return this.apiService.loginWithSso(provider, rememberMe).pipe(
      tap(res => this.storeSessionData(res, rememberMe)),
      map(res => res.user)
    );
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

    if (!this.apiService) {
      return throwError(() => new Error('API service unavailable'));
    }

    return this.apiService.updateLanguage(language, { headers }).pipe(
      tap(user => {
        this.currentUser.set(user);
      })
    );
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

    if (!this.apiService) {
      return throwError(() => new Error('API service unavailable'));
    }

    return this.apiService.updateProfile(profile, { headers }).pipe(
      tap(user => {
        this.currentUser.set(user);
      })
    );
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

    if (!this.apiService) {
      return throwError(() => new Error('API service unavailable'));
    }

    return this.apiService.updateEmail(newEmail, { headers }).pipe(
      tap(user => {
        this.currentUser.set(user);
      })
    );
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
}
