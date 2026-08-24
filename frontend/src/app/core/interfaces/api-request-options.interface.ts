import { HttpHeaders, HttpParams } from '@angular/common/http';

export interface ApiRequestOptions {
  headers?: HttpHeaders;
  params?: HttpParams;
  blockUi?: boolean;
}
