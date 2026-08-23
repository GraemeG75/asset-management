import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatRadioModule } from '@angular/material/radio';
import { BaseFormInputComponent } from '../base/form-input-base';

@Component({
  selector: 'gp-am-radio-input',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatRadioModule
  ],
  template: `
    <div class="field-container" [class.left-label]="isLeftLabel">
      <label class="label-text">
        {{ config.label }}
        <span *ngIf="isRequired && isEditable" class="required-asterisk">*</span>
      </label>

      <div class="input-wrapper">
        <mat-radio-group [formControl]="control" class="radio-group">
          <mat-radio-button *ngFor="let opt of config.options" [value]="opt.value" [disabled]="opt.disabled || isFieldDisabled" class="radio-button">
            {{ opt.label }}
          </mat-radio-button>
        </mat-radio-group>
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
    .label-text {
      font-weight: 600;
      color: var(--text-color, #334155);
      font-size: 0.95rem;
      margin-bottom: 8px;
    }
    @media (min-width: 640px) {
      .field-container.left-label {
        flex-direction: row;
        align-items: flex-start;
        gap: 16px;
      }
      .field-container.left-label .label-text {
        width: 160px;
        min-width: 140px;
        padding-top: 4px;
        text-align: right;
        margin-bottom: 0;
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
    .radio-group {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    @media (min-width: 480px) {
      .radio-group {
        flex-direction: row;
        flex-wrap: wrap;
        gap: 16px;
      }
    }
    .radio-button {
      margin-right: 8px;
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
export class RadioInputComponent extends BaseFormInputComponent {
  get isRequired(): boolean {
    return !!this.config.validators?.some(v => v.type === 'required');
  }
}
