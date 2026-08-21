import { TestBed } from '@angular/core/testing';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { Router, UrlTree } from '@angular/router';
import { authGuard } from './auth.guard';
import { UserService } from '../services/user.service';

describe('authGuard', () => {
  let userServiceMock: any;
  let routerMock: any;

  beforeEach(() => {
    userServiceMock = {
      isLoggedIn: vi.fn()
    };
    routerMock = {
      createUrlTree: vi.fn().mockImplementation((path, extras) => ({ path, extras }))
    };

    TestBed.configureTestingModule({
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    });
  });

  it('should allow activation when user is logged in', () => {
    userServiceMock.isLoggedIn.mockReturnValue(true);

    const result = TestBed.runInInjectionContext(() => authGuard({} as any, { url: '/protected' } as any));
    expect(result).toBe(true);
    expect(routerMock.createUrlTree).not.toHaveBeenCalled();
  });

  it('should redirect unauthenticated user to /login with returnUrl query param', () => {
    userServiceMock.isLoggedIn.mockReturnValue(false);

    const result = TestBed.runInInjectionContext(() => authGuard({} as any, { url: '/protected' } as any));
    expect(routerMock.createUrlTree).toHaveBeenCalledWith(['/login'], { queryParams: { returnUrl: '/protected' } });
    expect(result).toEqual({ path: ['/login'], extras: { queryParams: { returnUrl: '/protected' } } });
  });
});
