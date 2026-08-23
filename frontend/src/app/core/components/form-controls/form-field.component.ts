import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
import { FormFieldConfig, LabelPosition, FieldEmittedValue } from '../../models/form-schema.model';

import { TextInputComponent } from './inputs/text-input.component';
import { NumberInputComponent } from './inputs/number-input.component';
import { TextareaInputComponent } from './inputs/textarea-input.component';
import { SelectInputComponent } from './inputs/select-input.component';
import { RadioInputComponent } from './inputs/radio-input.component';
import { CheckboxInputComponent } from './inputs/checkbox-input.component';
import { DateInputComponent } from './inputs/date-input.component';
import { ToggleInputComponent } from './inputs/toggle-input.component';

@Component({
  selector: 'gp-am-form-field',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TextInputComponent,
    NumberInputComponent,
    TextareaInputComponent,
    SelectInputComponent,
    RadioInputComponent,
    CheckboxInputComponent,
    DateInputComponent,
    ToggleInputComponent
  ],
  template: `
    <div [ngSwitch]="config.type" class="field-wrapper">
      <gp-am-text-input
        *ngSwitchCase="'text'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-text-input>

      <gp-am-text-input
        *ngSwitchCase="'email'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-text-input>

      <gp-am-text-input
        *ngSwitchCase="'password'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-text-input>

      <gp-am-number-input
        *ngSwitchCase="'number'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-number-input>

      <gp-am-textarea-input
        *ngSwitchCase="'textarea'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-textarea-input>

      <gp-am-select-input
        *ngSwitchCase="'select'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-select-input>

      <gp-am-radio-input
        *ngSwitchCase="'radio'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-radio-input>

      <gp-am-checkbox-input
        *ngSwitchCase="'checkbox'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-checkbox-input>

      <gp-am-date-input
        *ngSwitchCase="'date'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-date-input>

      <gp-am-toggle-input
        *ngSwitchCase="'toggle'"
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-toggle-input>

      <gp-am-text-input
        *ngSwitchDefault
        [config]="config"
        [control]="control"
        [isEditable]="isEditable"
        [labelPosition]="effectiveLabelPosition"
        (valueEmitted)="onValueEmitted($event)"
      ></gp-am-text-input>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
    }
    .field-wrapper {
      width: 100%;
    }
  `]
})
export class FormFieldComponent {
  @Input({ required: true }) config!: FormFieldConfig;
  @Input({ required: true }) formGroup!: FormGroup;
  @Input() isEditable: boolean = true;
  @Input() labelPosition?: LabelPosition;

  @Output() valueEmitted = new EventEmitter<FieldEmittedValue>();

  get control(): FormControl {
    return (this.formGroup.get(this.config.key) as FormControl) || new FormControl('');
  }

  get effectiveLabelPosition(): LabelPosition {
    if (this.config.labelPosition) {
      return this.config.labelPosition;
    }
    if (this.labelPosition) {
      return this.labelPosition;
    }
    // Readonly -> 'top', Editable -> 'left'
    return this.isEditable ? 'left' : 'top';
  }

  onValueEmitted(event: FieldEmittedValue): void {
    this.valueEmitted.emit(event);
  }
}
