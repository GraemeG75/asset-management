# Frontend Localization (i18n) Rules

## 1. Localized UI Text
- All user-facing text strings displayed in the frontend components and templates must be localized using translation keys instead of hardcoded strings.

## 2. Dual Translation Endpoints
- Translation dictionaries are served dynamically from two backend endpoints:
  1. **Unauthenticated / Public Endpoint**: Returns dictionary key-values for unauthenticated views (e.g., Login, Password Reset, Landing page).
  2. **Authenticated Endpoint**: Returns dictionary key-values for authenticated views (e.g., Dashboard, Asset Management, User Profile, Settings).

## 3. Dynamic Translation Loading
- The Angular `TranslationService` automatically loads the public dictionary on application initialization.
- Upon successful login, the `TranslationService` switches to or merges the authenticated translation dictionary.
- Components use a custom `TranslatePipe` (`{{ 'KEY' | translate }}`) or `TranslationService` signal to reactively render localized strings.
