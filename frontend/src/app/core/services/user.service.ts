import { Injectable, signal, computed } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { User, LoginCredentials, SessionInfo, JwtPayload, AuthResponse } from '../models/user.model';

const TOKEN_KEY = 'asset_mgmt_jwt_token';
const REMEMBER_KEY = 'asset_mgmt_remember_me';

@Injectable({
  providedIn: 'root'
})
export class UserService {
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
      avatarUrl: payload.avatarUrl
    };

    this.jwtToken.set(token);
    this.currentUser.set(user);
    this.isRemembered.set(remembered);
    return true;
  }

  /**
   * Performs user login with credentials
   */
  login(credentials: LoginCredentials): Observable<User> {
    if (!credentials.email) {
      return throwError(() => new Error('Email is required'));
    }

    const rememberMe = credentials.rememberMe ?? true;
    const response = this.createMockAuthResponse(credentials.email);
    
    return of(response).pipe(
      tap(res => {
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
      }),
      map(res => res.user)
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
  createMockAuthResponse(email: string): AuthResponse {
    const isManager = email.includes('admin') || email.includes('manager');
    const isSpecial = email.includes('admin');
    
    const role: 'admin' | 'manager' | 'user' = isSpecial ? 'admin' : (isManager ? 'manager' : 'user');
    const name = email.split('@')[0].replace(/[._]/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
    
    const user: User = {
      id: `usr_${Math.random().toString(36).substring(2, 9)}`,
      name: name || 'Demo User',
      email: email,
      role: role,
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
