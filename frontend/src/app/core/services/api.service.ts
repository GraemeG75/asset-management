import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoadingService } from './loading.service';

export interface ApiRequestOptions {
  headers?: HttpHeaders;
  params?: HttpParams;
  blockUi?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient, { optional: true });
  private loadingService = inject(LoadingService);

  /**
   * Executes an HTTP GET request with optional UI blocking
   */
  get<T>(url: string, options: ApiRequestOptions = {}): Observable<T> {
    return this.executeRequest<T>(
      (opt) => this.http?.get<T>(url, opt) ?? new Observable<T>(),
      options
    );
  }

  /**
   * Executes an HTTP POST request with optional UI blocking
   */
  post<T>(url: string, body: any, options: ApiRequestOptions = {}): Observable<T> {
    return this.executeRequest<T>(
      (opt) => this.http?.post<T>(url, body, opt) ?? new Observable<T>(),
      options
    );
  }

  /**
   * Executes an HTTP PUT request with optional UI blocking
   */
  put<T>(url: string, body: any, options: ApiRequestOptions = {}): Observable<T> {
    return this.executeRequest<T>(
      (opt) => this.http?.put<T>(url, body, opt) ?? new Observable<T>(),
      options
    );
  }

  /**
   * Executes an HTTP DELETE request with optional UI blocking
   */
  delete<T>(url: string, options: ApiRequestOptions = {}): Observable<T> {
    return this.executeRequest<T>(
      (opt) => this.http?.delete<T>(url, opt) ?? new Observable<T>(),
      options
    );
  }

  /**
   * Internal wrapper that manages UI blocking via LoadingService
   */
  private executeRequest<T>(
    requestFn: (opt: { headers?: HttpHeaders; params?: HttpParams }) => Observable<T>,
    options: ApiRequestOptions
  ): Observable<T> {
    const shouldBlock = options.blockUi ?? true;

    if (shouldBlock) {
      this.loadingService.blockUi();
    }

    const httpOptions = {
      headers: options.headers,
      params: options.params
    };

    return requestFn(httpOptions).pipe(
      finalize(() => {
        if (shouldBlock) {
          this.loadingService.unblockUi();
        }
      })
    );
  }
}
