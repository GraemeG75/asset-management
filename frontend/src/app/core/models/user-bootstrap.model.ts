import { FormFieldConfig } from './form-schema.model';

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

export interface WidgetConfig {
  widgetType: 'kpi' | 'chart' | 'action';
  metricValue?: string;
  metricTrend?: string;
  trendDirection: 'up' | 'down' | 'neutral';
  accentColor: string;
  icon?: string;
}

export interface GridConfig {
  pageSize: number;
  allowSorting: boolean;
  allowPaging: boolean;
  rows: Record<string, any>[];
}

export interface SearchConfig {
  targetGridId?: string;
  autoSubmitOnReset: boolean;
  submitButtonLabel: string;
}

export type DashboardFormType = 'widget' | 'detail' | 'grid' | 'search' | 'standard';

export interface DashboardFormMetadata {
  formId: string;
  formType: DashboardFormType;
  caption: string;
  title: string;
  description?: string;
  formInfo?: string;
  isEditable: boolean;
  labelPosition?: 'left' | 'top';
  gridCols: number;
  fields: FormFieldConfig[];
  widgetConfig?: WidgetConfig;
  gridConfig?: GridConfig;
  searchConfig?: SearchConfig;
  submitButtonText?: string;
  showResetButton?: boolean;
}

export interface UserBootstrapData {
  userId: string;
  userName: string;
  userEmail: string;
  role: string;
  profileNavLinks: ProfileNavLink[];
  siteNavLinks: SiteNavLink[];
  inboxCount: number;
  dashboardForms: DashboardFormMetadata[];
}
