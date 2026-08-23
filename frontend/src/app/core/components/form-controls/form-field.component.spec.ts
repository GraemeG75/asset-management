import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormControl, FormGroup } from '@angular/forms';
import { provideAnimations } from '@angular/platform-browser/animations';
import { vi } from 'vitest';
import { FormFieldComponent } from './form-field.component';
import { FormFieldConfig } from '../../models/form-schema.model';

describe('FormFieldComponent', () => {
  let component: FormFieldComponent;
  let fixture: ComponentFixture<FormFieldComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormFieldComponent],
      providers: [provideAnimations()]
    }).compileComponents();

    fixture = TestBed.createComponent(FormFieldComponent);
    component = fixture.componentInstance;
  });

  it('should create form field component', () => {
    const config: FormFieldConfig = {
      key: 'testKey',
      label: 'Test Label',
      type: 'text'
    };
    const formGroup = new FormGroup({
      testKey: new FormControl('')
    });

    component.config = config;
    component.formGroup = formGroup;
    fixture.detectChanges();

    expect(component).toBeTruthy();
  });

  it('should forward value emissions from input components', () => {
    const config: FormFieldConfig = {
      key: 'testKey',
      label: 'Test Label',
      type: 'text'
    };
    const formGroup = new FormGroup({
      testKey: new FormControl('')
    });

    component.config = config;
    component.formGroup = formGroup;
    fixture.detectChanges();

    const spy = vi.spyOn(component.valueEmitted, 'emit');

    component.onValueEmitted({
      key: 'testKey',
      value: 'Hello World',
      valid: true
    });

    expect(spy).toHaveBeenCalledWith({
      key: 'testKey',
      value: 'Hello World',
      valid: true
    });
  });

  it('should place labels on top when form is read-only and to the left when editable', () => {
    const config: FormFieldConfig = {
      key: 'testKey',
      label: 'Test Label',
      type: 'text'
    };
    const formGroup = new FormGroup({
      testKey: new FormControl('')
    });

    component.config = config;
    component.formGroup = formGroup;

    component.isEditable = false;
    expect(component.effectiveLabelPosition).toBe('top');

    component.isEditable = true;
    expect(component.effectiveLabelPosition).toBe('left');
  });
});
