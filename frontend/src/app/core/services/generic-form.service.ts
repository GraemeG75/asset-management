import { Injectable, inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { ApiService } from './api.service';
import { ApiRequestOptions } from '../interfaces/api-request-options.interface';
import { DynamicFormType } from '../types/dynamic-form-type.type';
import { FormSubmissionRequest } from '../interfaces/form-submission-request.interface';
import { FormFieldError } from '../interfaces/form-field-error.interface';
import { FormSubmissionResult } from '../interfaces/form-submission-result.interface';

export type { DynamicFormType } from '../types/dynamic-form-type.type';
export type { FormSubmissionRequest } from '../interfaces/form-submission-request.interface';
export type { FormFieldError } from '../interfaces/form-field-error.interface';
export type { FormSubmissionResult } from '../interfaces/form-submission-result.interface';

@Injectable({
  providedIn: 'root'
})
export class GenericFormService {
  private apiService = inject(ApiService, { optional: true });

  /**
   * Unified form submission method covering all form types
   */
  submitForm(request: FormSubmissionRequest, options: ApiRequestOptions = {}): Observable<FormSubmissionResult> {
    if (!request || !request.formKey) {
      return throwError(() => new Error('Invalid form submission request: formKey is required'));
    }

    if (!this.apiService) {
      return throwError(() => new Error('API service is unavailable'));
    }

    return this.apiService.submitFormData<FormSubmissionResult>(request, options);
  }

  /**
   * Submits a Standard metadata form (e.g. preferences, settings)
   */
  submitStandardForm(
    pageKey: string, 
    formKey: string, 
    fieldValues: Record<string, any>, 
    recordId?: string, 
    options?: ApiRequestOptions
  ): Observable<FormSubmissionResult> {
    return this.submitForm({
      pageKey,
      formKey,
      formType: 'standard',
      recordId,
      action: 'save',
      fieldValues
    }, options);
  }

  /**
   * Submits a Detail single-record edit form
   */
  submitDetailForm(
    pageKey: string, 
    formKey: string, 
    recordId: string, 
    fieldValues: Record<string, any>, 
    options?: ApiRequestOptions
  ): Observable<FormSubmissionResult> {
    return this.submitForm({
      pageKey,
      formKey,
      formType: 'detail',
      recordId,
      action: 'update',
      fieldValues
    }, options);
  }

  /**
   * Submits a Grid form operation ('create', 'update', or 'delete')
   */
  submitGridForm(
    pageKey: string, 
    formKey: string, 
    action: 'create' | 'update' | 'delete', 
    fieldValues: Record<string, any>, 
    recordId?: string, 
    options?: ApiRequestOptions
  ): Observable<FormSubmissionResult> {
    return this.submitForm({
      pageKey,
      formKey,
      formType: 'grid',
      recordId,
      action,
      fieldValues
    }, options);
  }

  /**
   * Submits a Search/Filter criteria form
   */
  submitSearchForm(
    pageKey: string, 
    formKey: string, 
    searchCriteria: Record<string, any>, 
    options?: ApiRequestOptions
  ): Observable<FormSubmissionResult> {
    return this.submitForm({
      pageKey,
      formKey,
      formType: 'search',
      action: 'search',
      fieldValues: searchCriteria
    }, options);
  }

  /**
   * Submits a Dashboard Widget action form
   */
  submitWidgetAction(
    pageKey: string, 
    formKey: string, 
    widgetData: Record<string, any>, 
    options?: ApiRequestOptions
  ): Observable<FormSubmissionResult> {
    return this.submitForm({
      pageKey,
      formKey,
      formType: 'widget',
      action: 'widget-action',
      fieldValues: widgetData
    }, options);
  }
}
