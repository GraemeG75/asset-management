import { SsoProviderId } from '../types/sso-provider-id.type';

export interface SsoProvider {
  id: SsoProviderId;
  name: string;
  icon: string;
  color: string;
}
