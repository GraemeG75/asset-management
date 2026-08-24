import { TestBed } from '@angular/core/testing';
import { GenericFormService } from './generic-form.service';
import { ApiService } from './api.service';
import { of } from 'rxjs';

describe('GenericFormService', () => {
  let service: GenericFormService;
  let apiServiceSpy: jasmine.SpyObj<ApiService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('ApiService', ['submitFormData']);
    TestBed.configureTestingModule({
      providers: [
        GenericFormService,
        { provide: ApiService, useValue: spy }
      ]
    });
    service = TestBed.inject(GenericFormService);
    apiServiceSpy = TestBed.inject(ApiService) as jasmine.SpyObj<ApiService>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should submit standard form using unified API endpoint', (done) => {
    const mockResponse = {
      success: true,
      message: 'Form saved',
      formKey: 'profile-form',
      formType: 'standard' as const,
      data: { name: 'Test' }
    };
    apiServiceSpy.submitFormData.and.returnValue(of(mockResponse));

    service.submitStandardForm('profile', 'profile-form', { name: 'Test' }).subscribe((res) => {
      expect(res.success).toBeTrue();
      expect(res.formType).toBe('standard');
      expect(apiServiceSpy.submitFormData).toHaveBeenCalledWith(jasmine.objectContaining({
        pageKey: 'profile',
        formKey: 'profile-form',
        formType: 'standard',
        action: 'save'
      }), {});
      done();
    });
  });

  it('should submit grid form with action and recordId', (done) => {
    const mockResponse = {
      success: true,
      message: 'Grid row updated',
      recordId: '123',
      formKey: 'asset-grid',
      formType: 'grid' as const
    };
    apiServiceSpy.submitFormData.and.returnValue(of(mockResponse));

    service.submitGridForm('assets', 'asset-grid', 'update', { title: 'New Laser' }, '123').subscribe((res) => {
      expect(res.success).toBeTrue();
      expect(res.recordId).toBe('123');
      expect(apiServiceSpy.submitFormData).toHaveBeenCalledWith(jasmine.objectContaining({
        pageKey: 'assets',
        formKey: 'asset-grid',
        formType: 'grid',
        action: 'update',
        recordId: '123'
      }), {});
      done();
    });
  });

  it('should submit search form criteria', (done) => {
    const mockResponse = {
      success: true,
      message: 'Search executed',
      formKey: 'asset-search',
      formType: 'search' as const,
      data: { activeFilters: { query: 'laser' } }
    };
    apiServiceSpy.submitFormData.and.returnValue(of(mockResponse));

    service.submitSearchForm('assets', 'asset-search', { query: 'laser' }).subscribe((res) => {
      expect(res.success).toBeTrue();
      expect(res.formType).toBe('search');
      done();
    });
  });
});
