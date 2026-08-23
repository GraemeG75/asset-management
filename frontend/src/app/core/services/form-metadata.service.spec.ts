import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { FormMetadataService } from './form-metadata.service';
import { FormFieldConfig } from '../models/form-schema.model';

describe('FormMetadataService', () => {
  let service: FormMetadataService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        FormMetadataService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(FormMetadataService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create reactive form group with proper controls and initial values', () => {
    const fields: FormFieldConfig[] = [
      { key: 'name', label: 'Name', type: 'text', defaultValue: 'Test Asset' },
      { key: 'count', label: 'Count', type: 'number', defaultValue: 5 }
    ];

    const form = service.createFormGroup(fields);
    expect(form.contains('name')).toBe(true);
    expect(form.contains('count')).toBe(true);
    expect(form.get('name')?.value).toBe('Test Asset');
    expect(form.get('count')?.value).toBe(5);
  });

  it('should enforce validators on generated controls', () => {
    const fields: FormFieldConfig[] = [
      {
        key: 'email',
        label: 'Email',
        type: 'email',
        validators: [
          { type: 'required', message: 'Required' },
          { type: 'email', message: 'Invalid email' }
        ]
      }
    ];

    const form = service.createFormGroup(fields);
    const emailControl = form.get('email');

    emailControl?.setValue('');
    expect(emailControl?.valid).toBe(false);
    expect(emailControl?.hasError('required')).toBe(true);

    emailControl?.setValue('not-an-email');
    expect(emailControl?.valid).toBe(false);
    expect(emailControl?.hasError('email')).toBe(true);

    emailControl?.setValue('valid@example.com');
    expect(emailControl?.valid).toBe(true);
  });

  it('should fetch form schema from backend API', () => {
    service.getFormSchema('asset-create').subscribe(schema => {
      expect(schema.id).toBe('asset-create');
      expect(schema.fields.length).toBeGreaterThan(0);
    });

    const req = httpMock.expectOne('/api/form-metadata/asset-create');
    expect(req.request.method).toBe('GET');
    req.flush({
      id: 'asset-create',
      title: 'Asset Registration',
      labelPosition: 'left',
      fields: [{ key: 'name', label: 'Name', type: 'text' }]
    });
  });

  it('should propagate HTTP error when API request fails', () => {
    service.getFormSchema('user-profile').subscribe({
      next: () => expect.fail('should have failed with 500 error'),
      error: err => {
        expect(err.status).toBe(500);
      }
    });

    const req = httpMock.expectOne('/api/form-metadata/user-profile');
    req.flush('Error', { status: 500, statusText: 'Server Error' });
  });
});
