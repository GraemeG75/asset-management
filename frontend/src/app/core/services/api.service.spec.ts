import { TestBed } from '@angular/core/testing';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { ApiService, API_ENDPOINTS } from './api.service';
import { LoadingService } from './loading.service';

describe('ApiService', () => {
  let apiService: ApiService;
  let loadingService: LoadingService;
  let httpClientMock: any;

  beforeEach(() => {
    httpClientMock = {
      get: vi.fn().mockReturnValue(of({ success: true })),
      post: vi.fn().mockReturnValue(of({ success: true })),
      put: vi.fn().mockReturnValue(of({ success: true })),
      delete: vi.fn().mockReturnValue(of({ success: true }))
    };

    TestBed.configureTestingModule({
      providers: [
        ApiService,
        LoadingService,
        { provide: HttpClient, useValue: httpClientMock }
      ]
    });

    apiService = TestBed.inject(ApiService);
    loadingService = TestBed.inject(LoadingService);
  });

  it('should trigger UI blocking by default during GET request', () => {
    const blockSpy = vi.spyOn(loadingService, 'blockUi');
    const unblockSpy = vi.spyOn(loadingService, 'unblockUi');

    apiService.get('/test-endpoint').subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    expect(blockSpy).toHaveBeenCalled();
    expect(unblockSpy).toHaveBeenCalled();
  });

  it('should skip UI blocking when blockUi is false', () => {
    const blockSpy = vi.spyOn(loadingService, 'blockUi');
    const unblockSpy = vi.spyOn(loadingService, 'unblockUi');

    apiService.get('/test-endpoint', { blockUi: false }).subscribe();

    expect(blockSpy).not.toHaveBeenCalled();
    expect(unblockSpy).not.toHaveBeenCalled();
  });

  it('should route login to API_ENDPOINTS.AUTH_LOGIN', () => {
    apiService.login({ email: 'admin@assetmgmt.io', password: 'password123' }).subscribe();
    expect(httpClientMock.post).toHaveBeenCalledWith(
      API_ENDPOINTS.AUTH_LOGIN,
      { email: 'admin@assetmgmt.io', password: 'password123' },
      { headers: undefined, params: undefined }
    );
  });

  it('should route updateLanguage to API_ENDPOINTS.PROFILE_LANGUAGE', () => {
    apiService.updateLanguage('es').subscribe();
    expect(httpClientMock.put).toHaveBeenCalledWith(
      API_ENDPOINTS.PROFILE_LANGUAGE,
      { language: 'es' },
      { headers: undefined, params: undefined }
    );
  });

  it('should route getPublicTranslations to API_ENDPOINTS.TRANSLATIONS_PUBLIC', () => {
    apiService.getPublicTranslations('es').subscribe();
    expect(httpClientMock.get).toHaveBeenCalledWith(
      API_ENDPOINTS.TRANSLATIONS_PUBLIC,
      expect.objectContaining({ params: expect.any(Object) })
    );
  });
});
