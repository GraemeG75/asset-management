export interface ProfileNavLink {
  id: string;
  label: string;
  icon: string;
  url: string;
  badge?: string;
  badgeColor?: string;
  order: number;
  isActive: boolean;
}
