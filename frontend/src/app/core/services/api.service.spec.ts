import { TestBed } from '@angular/core/testing';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { ApiService } from './api.service';
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

  it('should call POST method with body and UI blocking', () => {
    apiService.post('/data', { payload: 123 }).subscribe();
    expect(httpClientMock.post).toHaveBeenCalledWith('/data', { payload: 123 }, { headers: undefined, params: undefined });
  });
});
