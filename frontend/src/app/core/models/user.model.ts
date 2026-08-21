export type SsoProviderId = 'google' | 'azure' | 'github';

export interface SsoProvider {
  id: SsoProviderId;
  name: string;
  icon: string;
  color: string;
}

export interface User {
  id: string;
  name: string;
  email: string;
  role: 'admin' | 'manager' | 'user';
  provider?: 'local' | SsoProviderId;
  avatarUrl?: string;
  createdAt?: string;
}

export interface LoginCredentials {
  email: string;
  password?: string;
  rememberMe?: boolean;
}

export interface AuthResponse {
  user: User;
  token: string;
  expiresAt: number;
}

export interface SessionInfo {
  isAuthenticated: boolean;
  user: User | null;
  token: string | null;
  loginTime: number | null;
  expiresAt: number | null;
  remembered: boolean;
}

export interface JwtPayload {
  sub: string;
  name: string;
  email: string;
  role: 'admin' | 'manager' | 'user';
  provider?: 'local' | SsoProviderId;
  avatarUrl?: string;
  iat: number;
  exp: number;
}
