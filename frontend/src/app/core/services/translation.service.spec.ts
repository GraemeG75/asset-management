import { TestBed } from '@angular/core';
import { TranslationService } from './translation.service';
import { UserService } from './user.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

describe('TranslationService', () => {
  let service: TranslationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TranslationService,
        UserService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(TranslationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should translate loaded keys', () => {
    service.translations.set({
      'LOGIN_TITLE': 'Sign in to AssetPulse',
      'SIGN_IN_BTN': 'Sign In'
    });
    expect(service.translate('LOGIN_TITLE')).toBe('Sign in to AssetPulse');
    expect(service.translate('SIGN_IN_BTN')).toBe('Sign In');
  });

  it('should return provided default text if key is missing', () => {
    expect(service.translate('NON_EXISTENT_KEY', 'Default Text')).toBe('Default Text');
  });
});
