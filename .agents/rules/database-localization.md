# Database Localization (i18n) & Schema Rules

## 1. Supported Locales Table (`locales`)
- Every database schema must include a central `locales` table storing all supported cultures/languages (e.g. `locale_code` PK: `en-US`, `es-ES`, `fr-FR`, `de-DE`).

## 2. Dual Table Pattern for User-Facing Entities (`<entity>` and `<entity>_locales`)
- Whenever a table contains user-facing information (labels, titles, descriptions, captions, help text, button labels, error messages), it MUST be split into:
  1. Base table (`<entity>`): Contains non-translatable metadata, primary keys, routes, order, layout configs, and flags.
  2. Localization table (`<entity>_locales`): Contains translatable text fields with composite primary key `(<entity_id>, locale_code)` referencing both `<entity>` and `locales(locale_code)`.

## 3. Localization Views with `en-US` Fallback
- Every localized entity MUST have a corresponding view (`vw_<entity>_localized`) that joins the base table with:
  - The requested locale translation table (`req_loc`).
  - The default `en-US` locale translation table (`def_loc`).
- The view must use `COALESCE(req_loc.column, def_loc.column)` so that if a translation for the requested locale is missing or NULL, it automatically falls back to `en-US`.

## 4. Primary & Foreign Keys (`UNIQUEIDENTIFIER`)
- ALL primary keys and foreign key relationships MUST use `UNIQUEIDENTIFIER` (e.g. `UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()`).
- Do NOT use auto-incrementing integer `INT`/`SERIAL` or string `VARCHAR` for entity keys.

## 5. String Data Types (`NVARCHAR`)
- ALL text and string columns MUST use `NVARCHAR(length)` or `NVARCHAR(MAX)` for full Unicode internationalization support.
- NEVER use `VARCHAR` or `TEXT`.
