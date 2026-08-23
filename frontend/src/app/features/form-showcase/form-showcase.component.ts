import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { FormComponent } from '../../core/components/form-controls/form.component';
import { FormMetadataService } from '../../core/services/form-metadata.service';
import { PageInfo, FormSchema, FieldEmittedValue } from '../../core/models/form-schema.model';

@Component({
  selector: 'gp-am-form-showcase',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatSnackBarModule,
    FormComponent
  ],
  templateUrl: './form-showcase.component.html',
  styleUrl: './form-showcase.component.css'
})
export class FormShowcaseComponent implements OnInit {
  private formService = inject(FormMetadataService);
  private snackBar = inject(MatSnackBar);

  availablePages: { pageId: string; title: string; description?: string }[] = [];
  selectedPageId: string = 'asset-operations';
  activePageInfo?: PageInfo;

  selectedFormId: string = 'asset-create';
  activeSchema?: FormSchema;
  
  // Form editable state
  isEditable: boolean = true;

  // Live state tracking
  liveFormValue: Record<string, any> = {};
  emittedEventsLog: { timestamp: string; key: string; value: any; valid: boolean }[] = [];
  formSubmittedValue?: Record<string, any>;

  ngOnInit(): void {
    this.loadAvailablePages();
  }

  loadAvailablePages(): void {
    this.formService.getAvailablePages().subscribe({
      next: pages => {
        this.availablePages = pages;
        if (pages.length > 0) {
          this.onPageIdChange(pages[0].pageId);
        }
      },
      error: () => {
        // Dev fallback if API server is not running
        this.availablePages = [
          { pageId: 'asset-operations', title: 'Asset Operations Workspace', description: 'Contains asset registration and maintenance forms' },
          { pageId: 'user-settings', title: 'User Account & Profile Page', description: 'Contains user settings and notification forms' },
          { pageId: 'audit-reports', title: 'Compliance & Audit Page', description: 'Contains read-only asset audit summary forms' }
        ];
        this.onPageIdChange('asset-operations');
      }
    });
  }

  /**
   * Step 1: When user selects a page, request Page Info (returns page title, description, and list of forms)
   */
  onPageIdChange(pageId: string): void {
    this.selectedPageId = pageId;
    this.formService.getPageInfo(pageId).subscribe({
      next: pageInfo => {
        this.activePageInfo = pageInfo;
        if (pageInfo.forms && pageInfo.forms.length > 0) {
          this.onFormIdChange(pageInfo.forms[0].formId);
        }
      },
      error: () => {
        this.loadFormSchema(this.selectedFormId);
      }
    });
  }

  /**
   * Step 2: Request individual Form Info containing caption, form info, and component metadata
   */
  onFormIdChange(formId: string): void {
    this.selectedFormId = formId;
    this.loadFormSchema(formId);
  }

  loadFormSchema(formId: string): void {
    this.formService.getFormSchema(formId).subscribe({
      next: schema => {
        this.activeSchema = schema;
        this.isEditable = schema.isEditable ?? true;
        this.liveFormValue = {};
        this.emittedEventsLog = [];
        this.formSubmittedValue = undefined;
      }
    });
  }

  setEditable(editable: boolean): void {
    this.isEditable = editable;
  }

  onFormValueChange(value: Record<string, any>): void {
    this.liveFormValue = value;
  }

  onFieldValueEmitted(event: FieldEmittedValue): void {
    const timeStr = new Date().toLocaleTimeString();
    this.emittedEventsLog.unshift({
      timestamp: timeStr,
      key: event.key,
      value: event.value,
      valid: event.valid
    });
    if (this.emittedEventsLog.length > 25) {
      this.emittedEventsLog.pop();
    }
  }

  onFormSubmit(submittedValue: Record<string, any>): void {
    this.formSubmittedValue = submittedValue;
    this.snackBar.open('Form successfully submitted!', 'Close', {
      duration: 3500,
      panelClass: ['success-snackbar']
    });
  }

  get jsonFormValue(): string {
    return JSON.stringify(this.liveFormValue, null, 2);
  }

  get jsonSubmittedValue(): string {
    return this.formSubmittedValue ? JSON.stringify(this.formSubmittedValue, null, 2) : '';
  }

  get jsonSchema(): string {
    return this.activeSchema ? JSON.stringify(this.activeSchema, null, 2) : '';
  }

  get jsonPageInfo(): string {
    return this.activePageInfo ? JSON.stringify(this.activePageInfo, null, 2) : '';
  }
}
