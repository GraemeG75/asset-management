import { Directive, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subscription } from 'rxjs';
import { FormFieldConfig, LabelPosition, FieldEmittedValue } from '../../../models/form-schema.model';

@Directive()
export abstract class BaseFormInputComponent implements OnInit, OnDestroy {
  @Input({ required: true }) config!: FormFieldConfig;
  @Input({ required: true }) control!: FormControl;
  @Input() isEditable: boolean = true;
  @Input() labelPosition?: LabelPosition;

  @Output() valueEmitted = new EventEmitter<FieldEmittedValue>();

  protected sub?: Subscription;

  ngOnInit(): void {
    if (this.control) {
      if (!this.isEditable || this.config.readonly || this.config.disabled) {
        this.control.disable({ emitEvent: false });
      }
      this.sub = this.control.valueChanges.subscribe(value => {
        this.emitValue(value);
      });
    }
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  protected emitValue(val: any): void {
    this.valueEmitted.emit({
      key: this.config.key,
      value: val,
      valid: this.control.valid
    });
  }

  get effectiveLabelPosition(): LabelPosition {
    // 1. Explicit field config override
    if (this.config.labelPosition) {
      return this.config.labelPosition;
    }
    // 2. Explicit component level labelPosition binding
    if (this.labelPosition) {
      return this.labelPosition;
    }
    // 3. Form mode rule: when readonly -> 'top', when editable -> 'left'
    return this.isEditable ? 'left' : 'top';
  }

  get isLeftLabel(): boolean {
    return this.effectiveLabelPosition === 'left';
  }

  get isFieldReadonly(): boolean {
    return !this.isEditable || !!this.config.readonly;
  }

  get isFieldDisabled(): boolean {
    return !this.isEditable || !!this.config.disabled;
  }

  get errorMessage(): string {
    if (!this.control || !this.control.errors || !this.control.touched) {
      return '';
    }

    if (this.config.validators) {
      for (const v of this.config.validators) {
        if (v.type === 'required' && this.control.hasError('required')) {
          return v.message || `${this.config.label} is required.`;
        }
        if (v.type === 'email' && this.control.hasError('email')) {
          return v.message || 'Invalid email address.';
        }
        if (v.type === 'minLength' && this.control.hasError('minlength')) {
          return v.message || `${this.config.label} must be at least ${v.value} characters.`;
        }
        if (v.type === 'maxLength' && this.control.hasError('maxlength')) {
          return v.message || `${this.config.label} cannot exceed ${v.value} characters.`;
        }
        if (v.type === 'min' && this.control.hasError('min')) {
          return v.message || `${this.config.label} must be at least ${v.value}.`;
        }
        if (v.type === 'max' && this.control.hasError('max')) {
          return v.message || `${this.config.label} cannot exceed ${v.value}.`;
        }
        if (v.type === 'pattern' && this.control.hasError('pattern')) {
          return v.message || `${this.config.label} format is invalid.`;
        }
      }
    }

    if (this.control.hasError('required')) return `${this.config.label} is required.`;
    if (this.control.hasError('email')) return 'Invalid email format.';
    if (this.control.hasError('pattern')) return 'Invalid pattern format.';

    return 'Invalid field value.';
  }
}
