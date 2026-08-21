# Angular API Client & UI Blocking Rules

## 1. Centralized API Service (`ApiService`)
- All HTTP API requests across Angular services and components must be executed through a centralized `ApiService` rather than invoking `HttpClient` directly in scattered locations.
- `ApiService` wraps standard HTTP methods (`get`, `post`, `put`, `delete`, `patch`) and provides unified headers, error handling, and UI state tracking.

## 2. Global UI Blocking & Loading (`LoadingService`)
- The `ApiService` integrates with a global `LoadingService` to manage an active request counter and a reactive `isBlocked` signal.
- API requests can accept an options parameter (e.g. `{ blockUi: true }`) to specify whether the call should trigger global UI blocking.
- When `isBlocked()` is `true`, a global `UiBlockerComponent` renders a full-screen frosted glass backdrop with a spinner, preventing user interaction until all blocking pending requests complete.
