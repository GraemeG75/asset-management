import { DynamicFormType } from '../types/dynamic-form-type.type';

export interface FormSubmissionRequest {
  pageKey: string;
  formKey: string;
  formType?: DynamicFormType;
  recordId?: string;
  action?: string;
  fieldValues: Record<string, any>;
}
