import { DynamicFormType } from '../types/dynamic-form-type.type';
import { FormFieldError } from './form-field-error.interface';

export interface FormSubmissionResult {
  success: boolean;
  message: string;
  recordId?: string;
  formKey: string;
  formType: DynamicFormType;
  data?: Record<string, any>;
  fieldErrors?: FormFieldError[];
}
