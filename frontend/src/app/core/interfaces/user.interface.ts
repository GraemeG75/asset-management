import { SsoProviderId } from '../types/sso-provider-id.type';

export interface User {
  id: string;
  firstName?: string;
  lastName?: string;
  name: string;
  email: string;
  role: number;
  provider?: 'local' | SsoProviderId;
  avatarUrl?: string;
  preferredLanguage?: string;
  dateCreated?: string;
}
