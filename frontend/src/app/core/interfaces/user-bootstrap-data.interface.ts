import { ProfileNavLink } from './profile-nav-link.interface';
import { SiteNavLink } from './site-nav-link.interface';
import { DashboardFormMetadata } from './dashboard-form-metadata.interface';

export interface UserBootstrapData {
  userId: string;
  userName: string;
  userEmail: string;
  role: number;
  profileNavLinks: ProfileNavLink[];
  siteNavLinks: SiteNavLink[];
  inboxCount: number;
  dashboardForms: DashboardFormMetadata[];
}
