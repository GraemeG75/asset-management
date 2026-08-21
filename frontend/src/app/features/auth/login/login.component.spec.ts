import { TestBed, ComponentFixture } from '@angular/core/testing';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { Router, ActivatedRoute } from '@angular/router';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { UserService } from '../../../core/services/user.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let userServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    userServiceMock = {
      isLoggedIn: vi.fn().mockReturnValue(false),
      login: vi.fn(),
      loginWithSso: vi.fn()
    };
    routerMock = {
      navigateByUrl: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideNoopAnimations(),
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              queryParams: { returnUrl: '/dashboard' }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the LoginComponent', () => {
    expect(component).toBeTruthy();
    expect(component.loginForm).toBeDefined();
    expect(component.loginForm.get('rememberMe')?.value).toBe(true);
  });

  it('should invalidate empty form submission', () => {
    component.onSubmit();
    expect(component.loginForm.valid).toBe(false);
    expect(userServiceMock.login).not.toHaveBeenCalled();
  });

  it('should call userService.login and navigate to returnUrl on successful submission', () => {
    userServiceMock.login.mockReturnValue(of({ email: 'admin@assetmgmt.io', name: 'Admin' }));
    component.loginForm.patchValue({ email: 'admin@assetmgmt.io', password: 'password123', rememberMe: true });

    component.onSubmit();

    expect(userServiceMock.login).toHaveBeenCalledWith({ email: 'admin@assetmgmt.io', password: 'password123', rememberMe: true });
    expect(routerMock.navigateByUrl).toHaveBeenCalledWith('/dashboard');
  });

  it('should trigger SSO login and navigate on success', () => {
    userServiceMock.loginWithSso.mockReturnValue(of({ email: 'alex.dev@gmail.com', provider: 'google' }));

    component.onSsoLogin('google');

    expect(userServiceMock.loginWithSso).toHaveBeenCalledWith('google', true);
    expect(routerMock.navigateByUrl).toHaveBeenCalledWith('/dashboard');
  });

  it('should display error message on failed login', () => {
    userServiceMock.login.mockReturnValue(throwError(() => new Error('Invalid email or password')));
    component.loginForm.patchValue({ email: 'invalid@assetmgmt.io', password: 'wrong' });

    component.onSubmit();

    expect(component.errorMessage()).toBe('Invalid email or password');
    expect(routerMock.navigateByUrl).not.toHaveBeenCalled();
  });
});
