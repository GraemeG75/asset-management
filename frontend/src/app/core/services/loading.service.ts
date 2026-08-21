import { Injectable, signal, computed } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private readonly activeRequests = signal<number>(0);

  /**
   * True if there is at least one active UI-blocking HTTP request
   */
  readonly isBlocked = computed<boolean>(() => this.activeRequests() > 0);

  /**
   * Increments active blocking request count
   */
  blockUi(): void {
    this.activeRequests.update(count => count + 1);
  }

  /**
   * Decrements active blocking request count
   */
  unblockUi(): void {
    this.activeRequests.update(count => Math.max(0, count - 1));
  }

  /**
   * Resets blocking state
   */
  reset(): void {
    this.activeRequests.set(0);
  }
}
