import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BaseFormInputComponent } from '../base/form-input-base';

@Component({
  selector: 'gp-am-number-input',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule
  ],
  template: `
    <div class="field-container" [class.left-label]="isLeftLabel">
      <label *ngIf="isLeftLabel" [for]="config.key" class="left-label-text">
        {{ config.label }}
        <span *ngIf="isRequired && isEditable" class="required-asterisk">*</span>
      </label>

      <div class="input-wrapper">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label *ngIf="!isLeftLabel">{{ config.label }}</mat-label>
          <input
            matInput
            type="number"
            [id]="config.key"
            [formControl]="control"
            [placeholder]="config.placeholder || ''"
            [readonly]="isFieldReadonly"
            [attr.min]="minVal"
            [attr.max]="maxVal"
          />
          <mat-hint *ngIf="config.helpText && !errorMessage">{{ config.helpText }}</mat-hint>
          <mat-error *ngIf="errorMessage">{{ errorMessage }}</mat-error>
        </mat-form-field>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
    }
    .field-container {
      display: flex;
      flex-direction: column;
      width: 100%;
    }
    .left-label-text {
      width: 100%;
      padding-bottom: 6px;
      font-weight: 600;
      color: var(--text-color, #334155);
      font-size: 0.95rem;
      text-align: left;
    }
    @media (min-width: 640px) {
      .field-container.left-label {
        flex-direction: row;
        align-items: flex-start;
        gap: 16px;
      }
      .field-container.left-label .left-label-text {
        width: 160px;
        min-width: 140px;
        padding-top: 14px;
        padding-bottom: 0;
        text-align: right;
      }
    }
    .required-asterisk {
      color: #e53935;
      margin-left: 2px;
    }
    .input-wrapper {
      flex: 1;
      width: 100%;
    }
    .full-width {
      width: 100%;
    }
  `]
})
export class NumberInputComponent extends BaseFormInputComponent {
  get isRequired(): boolean {
    return !!this.config.validators?.some(v => v.type === 'required');
  }

  get minVal(): number | null {
    const v = this.config.validators?.find(x => x.type === 'min');
    return v && typeof v.value === 'number' ? v.value : null;
  }

  get maxVal(): number | null {
    const v = this.config.validators?.find(x => x.type === 'max');
    return v && typeof v.value === 'number' ? v.value : null;
  }
}
