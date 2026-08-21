# Angular API Client & UI Blocking Rules

## 1. Centralized API Service & Endpoint Definitions (`ApiService`)
- All HTTP API endpoint URLs (e.g. `/auth/login`, `/auth/sso-login`, `/profile`, `/profile/email`, `/profile/language`, `/translations/public`, `/translations/authenticated`) must be stored in a single place inside `ApiService`.
- `ApiService` provides domain-specific strongly typed methods for each endpoint e.g.:
  - `login(credentials)`
  - `loginWithSso(provider, rememberMe)`
  - `getProfile()`
  - `updateProfile(dto)`
  - `updateEmail(newEmail)`
  - `updateLanguage(language)`
  - `getPublicTranslations(culture)`
  - `getAuthenticatedTranslations(culture, headers)`
- Domain services (`UserService`, `TranslationService`) must call these endpoint methods on `ApiService` instead of constructing API URLs or calling `HttpClient` directly.

## 2. Global UI Blocking & Loading (`LoadingService`)
- The `ApiService` integrates with a global `LoadingService` to manage an active request counter and a reactive `isBlocked` signal.
- API requests accept an options parameter (e.g. `{ blockUi: true }`) to specify whether the call should trigger global UI blocking.
- When `isBlocked()` is `true`, a global `UiBlockerComponent` renders a full-screen frosted glass backdrop with a spinner, preventing user interaction until all blocking pending requests complete.
