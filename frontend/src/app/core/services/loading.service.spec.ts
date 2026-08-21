import { describe, it, expect, beforeEach } from 'vitest';
import { LoadingService } from './loading.service';

describe('LoadingService', () => {
  let service: LoadingService;

  beforeEach(() => {
    service = new LoadingService();
  });

  it('should initialize with isBlocked false', () => {
    expect(service.isBlocked()).toBe(false);
  });

  it('should set isBlocked true when blockUi is called', () => {
    service.blockUi();
    expect(service.isBlocked()).toBe(true);
  });

  it('should maintain isBlocked true for multiple concurrent requests until all unblock', () => {
    service.blockUi();
    service.blockUi();
    expect(service.isBlocked()).toBe(true);

    service.unblockUi();
    expect(service.isBlocked()).toBe(true);

    service.unblockUi();
    expect(service.isBlocked()).toBe(false);
  });

  it('should reset state cleanly', () => {
    service.blockUi();
    service.blockUi();
    service.reset();
    expect(service.isBlocked()).toBe(false);
  });
});
