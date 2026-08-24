export interface SiteNavLink {
  id: string;
  label: string;
  icon: string;
  route: string;
  badgeCount?: number;
  category: string;
  order: number;
  isActive: boolean;
}
