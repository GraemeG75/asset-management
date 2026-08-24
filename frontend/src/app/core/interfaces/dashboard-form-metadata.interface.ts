import { DashboardFormType } from '../types/dashboard-form-type.type';
import { FormFieldConfig } from './form-field-config.interface';
import { WidgetConfig } from './widget-config.interface';
import { GridConfig } from './grid-config.interface';
import { SearchConfig } from './search-config.interface';

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
