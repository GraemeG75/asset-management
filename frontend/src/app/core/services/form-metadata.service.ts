import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators, ValidatorFn } from '@angular/forms';
import { Observable, forkJoin } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import {
  PageInfo,
  FormSchema,
  FormFieldConfig,
  FieldValidator
} from '../models/form-schema.model';

@Injectable({
  providedIn: 'root'
})
export class FormMetadataService {
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);
  private baseUrl = '/api/form-metadata';

  /**
   * Gets list of available pages in system
   */
  getAvailablePages(): Observable<{ pageId: string; title: string; description?: string }[]> {
    return this.http.get<{ pageId: string; title: string; description?: string }[]>(`${this.baseUrl}/pages`);
  }

  /**
   * Step 1: Fetches Page Info containing page title, description, and list of forms available on the page
   */
  getPageInfo(pageId: string): Observable<PageInfo> {
    return this.http.get<PageInfo>(`${this.baseUrl}/pages/${pageId}`);
  }

  /**
   * Step 2: Fetches individual Form Info by formId.
   * Returns complete schema containing caption, form info, editable state, and component field metadata.
   */
  getFormSchema(formId: string): Observable<FormSchema> {
    return this.http.get<FormSchema>(`${this.baseUrl}/${formId}`);
  }

  /**
   * Helper workflow: Fetches Page Info first, then fetches all form schemas available on that page
   */
  getFormsForPage(pageId: string): Observable<FormSchema[]> {
    return this.getPageInfo(pageId).pipe(
      switchMap(pageInfo => {
        if (!pageInfo.forms || pageInfo.forms.length === 0) {
          return [];
        }
        const requests = pageInfo.forms.map(formSummary => this.getFormSchema(formSummary.formId));
        return forkJoin(requests);
      })
    );
  }

  /**
   * Builds an Angular Reactive FormGroup based on field metadata and form editable state
   */
  createFormGroup(
    fields: FormFieldConfig[],
    initialValues: Record<string, any> = {},
    isFormEditable: boolean = true
  ): FormGroup {
    const group: Record<string, any> = {};

    fields.forEach(field => {
      const initialValue = initialValues[field.key] !== undefined
        ? initialValues[field.key]
        : (field.value !== undefined ? field.value : field.defaultValue ?? this.getDefaultValueForType(field.type));

      const validatorFns = this.buildValidators(field.validators);
      const isFieldDisabled = !isFormEditable || !!field.disabled;

      group[field.key] = [{
        value: initialValue,
        disabled: isFieldDisabled
      }, validatorFns];
    });

    return this.fb.group(group);
  }

  private buildValidators(validators: FieldValidator[] = []): ValidatorFn[] {
    const fns: ValidatorFn[] = [];

    validators.forEach(v => {
      switch (v.type) {
        case 'required':
          fns.push(Validators.required);
          break;
        case 'email':
          fns.push(Validators.email);
          break;
        case 'minLength':
          if (typeof v.value === 'number') {
            fns.push(Validators.minLength(v.value));
          }
          break;
        case 'maxLength':
          if (typeof v.value === 'number') {
            fns.push(Validators.maxLength(v.value));
          }
          break;
        case 'min':
          if (typeof v.value === 'number') {
            fns.push(Validators.min(v.value));
          }
          break;
        case 'max':
          if (typeof v.value === 'number') {
            fns.push(Validators.max(v.value));
          }
          break;
        case 'pattern':
          if (v.value) {
            fns.push(Validators.pattern(v.value));
          }
          break;
      }
    });

    return fns;
  }

  private getDefaultValueForType(type: string): any {
    switch (type) {
      case 'checkbox':
      case 'toggle':
        return false;
      case 'number':
        return null;
      default:
        return '';
    }
  }
}
