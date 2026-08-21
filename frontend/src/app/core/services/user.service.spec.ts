import { TestBed } from '@angular/core/testing';
import { describe, it, expect, beforeEach } from 'vitest';
import { firstValueFrom } from 'rxjs';
import { UserService } from './user.service';

describe('UserService', () => {
  let service: UserService;

  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();

    TestBed.configureTestingModule({
      providers: [UserService]
    });

    service = TestBed.inject(UserService);
  });

  it('should initialize with no user logged in', () => {
    expect(service.isLoggedIn()).toBe(false);
    expect(service.currentUser()).toBeNull();
    expect(service.userName()).toBe('Guest');
    expect(service.sessionInfo().isAuthenticated).toBe(false);
  });

  it('should login user with rememberMe=true and store in localStorage', async () => {
    const credentials = { email: 'admin@assetmgmt.io', password: 'password123', rememberMe: true };

    const user = await firstValueFrom(service.login(credentials));

    expect(user).toBeTruthy();
    expect(service.isLoggedIn()).toBe(true);
    expect(service.isRemembered()).toBe(true);
    expect(service.sessionInfo().remembered).toBe(true);
    expect(localStorage.getItem('asset_mgmt_jwt_token')).toBe(service.jwtToken());
    expect(sessionStorage.getItem('asset_mgmt_jwt_token')).toBeNull();
  });

  it('should login user with rememberMe=false and store in sessionStorage', async () => {
    const credentials = { email: 'user@assetmgmt.io', password: 'password123', rememberMe: false };

    const user = await firstValueFrom(service.login(credentials));

    expect(user).toBeTruthy();
    expect(service.isLoggedIn()).toBe(true);
    expect(service.isRemembered()).toBe(false);
    expect(service.sessionInfo().remembered).toBe(false);
    expect(sessionStorage.getItem('asset_mgmt_jwt_token')).toBe(service.jwtToken());
    expect(localStorage.getItem('asset_mgmt_jwt_token')).toBeNull();
  });

  it('should restore session from valid stored JWT in localStorage', () => {
    const mockAuth = service.createMockAuthResponse('manager@assetmgmt.io');
    localStorage.setItem('asset_mgmt_jwt_token', mockAuth.token);

    const restored = service.restoreSession();

    expect(restored).toBe(true);
    expect(service.isLoggedIn()).toBe(true);
    expect(service.isRemembered()).toBe(true);
    expect(service.currentUser()?.email).toBe('manager@assetmgmt.io');
  });

  it('should restore session from valid stored JWT in sessionStorage', () => {
    const mockAuth = service.createMockAuthResponse('guest@assetmgmt.io');
    sessionStorage.setItem('asset_mgmt_jwt_token', mockAuth.token);

    const restored = service.restoreSession();

    expect(restored).toBe(true);
    expect(service.isLoggedIn()).toBe(true);
    expect(service.isRemembered()).toBe(false);
    expect(service.currentUser()?.email).toBe('guest@assetmgmt.io');
  });

  it('should clear state and remove tokens on logout', async () => {
    await firstValueFrom(service.login({ email: 'user@assetmgmt.io', rememberMe: true }));
    expect(service.isLoggedIn()).toBe(true);

    service.logout();

    expect(service.isLoggedIn()).toBe(false);
    expect(service.currentUser()).toBeNull();
    expect(service.jwtToken()).toBeNull();
    expect(localStorage.getItem('asset_mgmt_jwt_token')).toBeNull();
    expect(sessionStorage.getItem('asset_mgmt_jwt_token')).toBeNull();
  });
});
