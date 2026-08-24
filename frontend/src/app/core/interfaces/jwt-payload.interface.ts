import { SsoProviderId } from '../types/sso-provider-id.type';

export interface JwtPayload {
  sub: string;
  name: string;
  email: string;
  role: number | string;
  provider?: 'local' | SsoProviderId;
  avatarUrl?: string;
  iat: number;
  exp: number;
}
