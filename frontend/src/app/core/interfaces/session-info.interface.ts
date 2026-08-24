import { User } from './user.interface';

export interface SessionInfo {
  isAuthenticated: boolean;
  user: User | null;
  token: string | null;
  loginTime: number | null;
  expiresAt: number | null;
  remembered: boolean;
}
