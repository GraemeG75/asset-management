import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { vi } from 'vitest';
import { FormComponent } from './form.component';
import { FormSchema, FieldEmittedValue } from '../../models/form-schema.model';

describe('FormComponent', () => {
  let component: FormComponent;
  let fixture: ComponentFixture<FormComponent>;

  const testSchema: FormSchema = {
    id: 'test-form',
    title: 'Test Form',
    isEditable: true,
    fields: [
      {
        key: 'assetName',
        label: 'Asset Name',
        type: 'text',
        validators: [{ type: 'required' }]
      },
      {
        key: 'category',
        label: 'Category',
        type: 'select',
        options: [{ label: 'Hardware', value: 'hw' }]
      }
    ]
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormComponent],
      providers: [
        provideAnimations(),
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(FormComponent);
    component = fixture.componentInstance;
    component.schema = testSchema;
    fixture.detectChanges();
  });

  it('should create form component with gp-am-form selector', () => {
    expect(component).toBeTruthy();
    expect(component.formGroup).toBeDefined();
    expect(component.formGroup.contains('assetName')).toBe(true);
  });

  it('should place labels on left when form is editable and on top when read-only', () => {
    component.isEditable = true;
    expect(component.effectiveLabelPosition).toBe('left');

    component.isEditable = false;
    expect(component.effectiveLabelPosition).toBe('top');
  });

  it('should disable all controls when form is read-only', () => {
    component.isEditable = false;
    component.ngOnChanges({
      isEditable: {
        currentValue: false,
        previousValue: true,
        firstChange: false,
        isFirstChange: () => false
      }
    });

    expect(component.formGroup.disabled).toBe(true);
  });

  it('should emit form values on submit when valid', () => {
    const spy = vi.spyOn(component.formSubmit, 'emit');

    component.formGroup.get('assetName')?.setValue('MacBook Pro');
    component.onSubmit();

    expect(spy).toHaveBeenCalledWith(
      expect.objectContaining({ assetName: 'MacBook Pro' })
    );
  });

  it('should emit field level values when child inputs change', () => {
    const spy = vi.spyOn(component.fieldValueEmitted, 'emit');

    const emittedPayload: FieldEmittedValue = {
      key: 'assetName',
      value: 'Dell Workstation',
      valid: true
    };

    component.onFieldEmitted(emittedPayload);
    expect(spy).toHaveBeenCalledWith(emittedPayload);
  });
});
