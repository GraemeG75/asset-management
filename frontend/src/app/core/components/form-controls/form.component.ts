import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  OnChanges,
  SimpleChanges,
  inject,
  OnDestroy
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Subscription } from 'rxjs';
import {
  FormSchema,
  FormFieldConfig,
  LabelPosition,
  FieldEmittedValue
} from '../../models/form-schema.model';
import { FormMetadataService } from '../../services/form-metadata.service';
import { FormFieldComponent } from './form-field.component';

@Component({
  selector: 'gp-am-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    FormFieldComponent
  ],
  templateUrl: './form.component.html',
  styleUrl: './form.component.css'
})
export class FormComponent implements OnInit, OnChanges, OnDestroy {
  private formService = inject(FormMetadataService);

  @Input() schema?: FormSchema;
  @Input() fields?: FormFieldConfig[];
  @Input() isEditable?: boolean; // When false, all components are readonly & labels on top
  @Input() labelPosition?: LabelPosition;
  @Input() initialValues: Record<string, any> = {};
  @Input() disabled: boolean = false;

  @Output() formSubmit = new EventEmitter<Record<string, any>>();
  @Output() formValueChange = new EventEmitter<Record<string, any>>();
  @Output() fieldValueEmitted = new EventEmitter<FieldEmittedValue>();

  formGroup!: FormGroup;
  private valueSub?: Subscription;

  ngOnInit(): void {
    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['schema'] || changes['fields'] || changes['initialValues'] || changes['isEditable']) {
      this.initForm();
    }
  }

  ngOnDestroy(): void {
    this.valueSub?.unsubscribe();
  }

  private initForm(): void {
    const activeFields = this.effectiveFields;
    const editableState = this.effectiveIsEditable;

    this.formGroup = this.formService.createFormGroup(activeFields, this.initialValues, editableState);

    if (this.disabled || !editableState) {
      this.formGroup.disable({ emitEvent: false });
    }

    this.valueSub?.unsubscribe();
    this.valueSub = this.formGroup.valueChanges.subscribe(val => {
      this.formValueChange.emit(val);
    });
  }

  get effectiveFields(): FormFieldConfig[] {
    if (this.fields && this.fields.length > 0) {
      return this.fields;
    }
    if (this.schema && this.schema.fields) {
      return this.schema.fields;
    }
    return [];
  }

  get formCaption(): string {
    return this.schema?.caption || this.schema?.title || '';
  }

  get effectiveIsEditable(): boolean {
    if (this.isEditable !== undefined) {
      return this.isEditable;
    }
    if (this.schema && this.schema.isEditable !== undefined) {
      return this.schema.isEditable;
    }
    return true;
  }

  get effectiveLabelPosition(): LabelPosition {
    // If component binding or schema explicitly sets labelPosition, honor it
    if (this.labelPosition) {
      return this.labelPosition;
    }
    if (this.schema && this.schema.labelPosition) {
      return this.schema.labelPosition;
    }
    // Rule: Readonly -> 'top', Editable -> 'left'
    return this.effectiveIsEditable ? 'left' : 'top';
  }

  get submitButtonText(): string {
    return this.schema?.submitButtonText || 'Submit';
  }

  get showResetButton(): boolean {
    return this.schema?.showResetButton ?? true;
  }

  onFieldEmitted(event: FieldEmittedValue): void {
    this.fieldValueEmitted.emit(event);
  }

  onSubmit(): void {
    if (this.formGroup.invalid && this.effectiveIsEditable) {
      this.formGroup.markAllAsTouched();
      return;
    }

    this.formSubmit.emit(this.formGroup.getRawValue());
  }

  onReset(): void {
    this.formGroup.reset();
  }

  public getRawValue(): Record<string, any> {
    return this.formGroup.getRawValue();
  }

  public isValid(): boolean {
    return this.formGroup.valid;
  }
}
