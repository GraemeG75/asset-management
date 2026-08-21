# Frontend Localization (i18n) Rules

## 1. Localized UI Text
- All user-facing text strings displayed in the frontend components and templates must be localized using translation keys instead of hardcoded strings.

## 2. Dual Translation Endpoints
- Translation dictionaries are served dynamically from two backend endpoints:
  1. **Unauthenticated / Public Endpoint**: Returns dictionary key-values for unauthenticated views (e.g., Login, Password Reset, Landing page).
  2. **Authenticated Endpoint**: Returns dictionary key-values for authenticated views (e.g., Dashboard, Asset Management, User Profile, Settings).

## 3. Dynamic Translation Loading & Language Switching
- The Angular `TranslationService` automatically loads the public dictionary on application initialization.
- The `LoginComponent` includes a language switcher control (`en`, `es`, `fr`, `de`) allowing unauthenticated users to switch cultures prior to logging in.
- Upon successful login, the `TranslationService` automatically synchronizes with the authenticated user's preferred language saved in their profile.

## 4. User Profile Language Persistence in DB
- Authenticated users manage their preferred language via their user profile.
- When an authenticated user changes their language, `UserService.updatePreferredLanguage(language)` calls `PUT /api/auth/profile/language` in the C# backend API.
- The backend persists `PreferredLanguage` in the database (`UserEntity.PreferredLanguage`) and returns the updated `UserDto`, ensuring language preferences persist across user sessions.
