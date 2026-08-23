import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { BaseFormInputComponent } from '../base/form-input-base';

@Component({
  selector: 'gp-am-checkbox-input',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCheckboxModule
  ],
  template: `
    <div class="field-container" [class.left-label]="isLeftLabel">
      <label *ngIf="isLeftLabel" class="left-label-text">
        {{ config.label }}
        <span *ngIf="isRequired && isEditable" class="required-asterisk">*</span>
      </label>

      <div class="input-wrapper">
        <mat-checkbox [formControl]="control" color="primary">
          <span *ngIf="!isLeftLabel">
            {{ config.label }}
            <span *ngIf="isRequired && isEditable" class="required-asterisk">*</span>
          </span>
        </mat-checkbox>
        <div class="help-text" *ngIf="config.helpText && !errorMessage">{{ config.helpText }}</div>
        <div class="error-text" *ngIf="errorMessage">{{ errorMessage }}</div>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
      margin-bottom: 16px;
    }
    .field-container {
      display: flex;
      flex-direction: column;
      width: 100%;
    }
    .left-label-text {
      width: 100%;
      padding-bottom: 4px;
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
        padding-top: 4px;
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
    .help-text {
      font-size: 0.75rem;
      color: #666;
      margin-top: 4px;
    }
    .error-text {
      font-size: 0.75rem;
      color: #f44336;
      margin-top: 4px;
    }
  `]
})
export class CheckboxInputComponent extends BaseFormInputComponent {
  get isRequired(): boolean {
    return !!this.config.validators?.some(v => v.type === 'required');
  }
}
