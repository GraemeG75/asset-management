import { LabelPosition } from '../types/label-position.type';
import { FormFieldConfig } from './form-field-config.interface';

export interface FormSchema {
  id: string;
  caption?: string;
  title?: string;
  description?: string;
  formInfo?: string;
  isEditable?: boolean;
  labelPosition?: LabelPosition;
  fields: FormFieldConfig[];
  submitButtonText?: string;
  showResetButton?: boolean;
}
