export type FieldType =
  | 'text'
  | 'email'
  | 'password'
  | 'number'
  | 'textarea'
  | 'select'
  | 'radio'
  | 'checkbox'
  | 'date'
  | 'toggle';

export type LabelPosition = 'top' | 'left';

export interface SelectOption {
  label: string;
  value: any;
  disabled?: boolean;
}

export interface FieldValidator {
  type: 'required' | 'minLength' | 'maxLength' | 'min' | 'max' | 'pattern' | 'email';
  value?: any;
  message?: string;
}

export interface FormFieldConfig {
  key: string;
  label: string;
  type: FieldType;
  placeholder?: string;
  defaultValue?: any;
  value?: any;
  labelPosition?: LabelPosition; // Field-level explicit override
  options?: SelectOption[];
  validators?: FieldValidator[];
  disabled?: boolean;
  readonly?: boolean;
  helpText?: string;
  gridCols?: number; // Span 1 to 12 columns for flex layout (default 12 for full width)
  customCssClass?: string;
}

export interface PageFormSummary {
  formId: string;
  caption: string;
  description?: string;
}

export interface PageInfo {
  pageId: string;
  title: string;
  description?: string;
  forms: PageFormSummary[];
}

export interface FormSchema {
  id: string;
  caption?: string; // Form caption
  title?: string; // Main title
  description?: string;
  formInfo?: string; // Form info / instructions
  isEditable?: boolean; // When false, all components are readonly and labels appear on top
  labelPosition?: LabelPosition; // Form-level override
  fields: FormFieldConfig[];
  submitButtonText?: string;
  showResetButton?: boolean;
}

export interface FieldEmittedValue {
  key: string;
  value: any;
  valid: boolean;
}
