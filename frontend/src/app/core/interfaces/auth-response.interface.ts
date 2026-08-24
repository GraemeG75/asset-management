import { User } from './user.interface';

export interface AuthResponse {
  user: User;
  token: string;
  expiresAt: number;
}
