import { FieldType } from '../types/field-type.type';
import { LabelPosition } from '../types/label-position.type';
import { SelectOption } from './select-option.interface';
import { FieldValidator } from './field-validator.interface';

export interface FormFieldConfig {
  key: string;
  label: string;
  type: FieldType;
  placeholder?: string;
  defaultValue?: any;
  value?: any;
  labelPosition?: LabelPosition;
  options?: SelectOption[];
  validators?: FieldValidator[];
  disabled?: boolean;
  readonly?: boolean;
  helpText?: string;
  gridCols?: number;
  customCssClass?: string;
}
