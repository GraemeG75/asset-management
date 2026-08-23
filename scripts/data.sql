-- ============================================================================
-- DATA.SQL
-- Seed Data Script for Master Locales, Navigation, Pages, Mappers, Flavors & Forms
-- Includes Visible Clauses on Forms & Localized Mapper Flavors
-- Rule: All metadata tables are prefixed with 'x_'
-- All primary/foreign keys use UNIQUEIDENTIFIER (valid RFC 4122 UUID v4 constants for seed data)
-- All string constants use N'...' NVARCHAR literals
-- Includes en-US (default fallback) and multilingual translations (es-ES, fr-FR, de-DE)
-- ============================================================================

-- 1. Seed Supported Master Locales
MERGE INTO x_locales AS target
USING (VALUES 
    (N'en-US', N'English (United States)', 1, 1),
    (N'es-ES', N'Español (España)', 0, 1),
    (N'fr-FR', N'Français (France)', 0, 1),
    (N'de-DE', N'Deutsch (Deutschland)', 0, 1)
) AS source (locale_code, display_name, is_default, is_active)
ON target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET display_name = source.display_name, is_default = source.is_default, is_active = source.is_active
WHEN NOT MATCHED THEN 
    INSERT (locale_code, display_name, is_default, is_active)
    VALUES (source.locale_code, source.display_name, source.is_default, source.is_active);
GO

-- 2. Seed Base Site Navigation Links & Locales
MERGE INTO x_site_nav_links AS target
USING (VALUES 
    (CAST('e8a719c2-570a-4a2e-9d2a-8d7d91e84321' AS UNIQUEIDENTIFIER), N'nav-dashboard', N'home', N'/dashboard', 4, N'Main', 1, 1),
    (CAST('a9b8c7d6-e5f4-4321-8765-43210fedcba9' AS UNIQUEIDENTIFIER), N'nav-assets', N'box', N'/assets', 12, N'Management', 2, 1),
    (CAST('b1c2d3e4-f5a6-4b7c-8d9e-0f1a2b3c4d5e' AS UNIQUEIDENTIFIER), N'nav-compliance', N'check-circle', N'/compliance', NULL, N'Management', 3, 1),
    (CAST('c2d3e4f5-a6b7-4c8d-9e0f-1a2b3c4d5e6f' AS UNIQUEIDENTIFIER), N'nav-audits', N'file-text', N'/audits', NULL, N'Archive', 4, 1),
    (CAST('d3e4f5a6-b7c8-4d9e-0f1a-2b3c4d5e6f7a' AS UNIQUEIDENTIFIER), N'nav-analytics', N'bar-chart', N'/analytics', NULL, N'Archive', 5, 1)
) AS source (id, link_key, icon, route, badge_count, category, display_order, is_active)
ON target.link_key = source.link_key
WHEN MATCHED THEN 
    UPDATE SET icon = source.icon, route = source.route, badge_count = source.badge_count, category = source.category, display_order = source.display_order
WHEN NOT MATCHED THEN 
    INSERT (id, link_key, icon, route, badge_count, category, display_order, is_active)
    VALUES (source.id, source.link_key, source.icon, source.route, source.badge_count, source.category, source.display_order, source.is_active);
GO

-- Site Nav Link Translations
MERGE INTO x_site_nav_link_locales AS target
USING (VALUES 
    (CAST('e8a719c2-570a-4a2e-9d2a-8d7d91e84321' AS UNIQUEIDENTIFIER), N'en-US', N'Inbox & Dashboard'),
    (CAST('a9b8c7d6-e5f4-4321-8765-43210fedcba9' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Operations'),
    (CAST('b1c2d3e4-f5a6-4b7c-8d9e-0f1a2b3c4d5e' AS UNIQUEIDENTIFIER), N'en-US', N'Compliance & Safety'),
    (CAST('c2d3e4f5-a6b7-4c8d-9e0f-1a2b3c4d5e6f' AS UNIQUEIDENTIFIER), N'en-US', N'Audit Archive'),
    (CAST('d3e4f5a6-b7c8-4d9e-0f1a-2b3c4d5e6f7a' AS UNIQUEIDENTIFIER), N'en-US', N'Reports & Analytics'),

    (CAST('e8a719c2-570a-4a2e-9d2a-8d7d91e84321' AS UNIQUEIDENTIFIER), N'es-ES', N'Bandeja de Entrada y Panel'),
    (CAST('a9b8c7d6-e5f4-4321-8765-43210fedcba9' AS UNIQUEIDENTIFIER), N'es-ES', N'Operaciones de Activos'),
    (CAST('b1c2d3e4-f5a6-4b7c-8d9e-0f1a2b3c4d5e' AS UNIQUEIDENTIFIER), N'es-ES', N'Cumplimiento y Seguridad'),
    (CAST('c2d3e4f5-a6b7-4c8d-9e0f-1a2b3c4d5e6f' AS UNIQUEIDENTIFIER), N'es-ES', N'Archivo de Auditorías'),
    (CAST('d3e4f5a6-b7c8-4d9e-0f1a-2b3c4d5e6f7a' AS UNIQUEIDENTIFIER), N'es-ES', N'Informes y Analítica')
) AS source (nav_id, locale_code, label)
ON target.nav_id = source.nav_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET label = source.label
WHEN NOT MATCHED THEN 
    INSERT (nav_id, locale_code, label) VALUES (source.nav_id, source.locale_code, source.label);
GO

-- 3. Seed Base Profile Navigation Links & Locales
MERGE INTO x_profile_nav_links AS target
USING (VALUES 
    (CAST('f47ac10b-58cc-4372-a567-0e02b2c3d479' AS UNIQUEIDENTIFIER), N'profile-settings', N'user', N'/profile', NULL, NULL, 1, 1),
    (CAST('e36ab09a-47bb-4261-9456-fdf1a1b2c368' AS UNIQUEIDENTIFIER), N'profile-security', N'shield', N'/profile/security', NULL, NULL, 2, 1),
    (CAST('d25ea989-36aa-4150-8345-ece090a1b257' AS UNIQUEIDENTIFIER), N'profile-preferences', N'sliders', N'/profile/preferences', NULL, NULL, 3, 1)
) AS source (id, link_key, icon, url, badge, badge_color, display_order, is_active)
ON target.link_key = source.link_key
WHEN MATCHED THEN 
    UPDATE SET icon = source.icon, url = source.url, badge = source.badge, badge_color = source.badge_color, display_order = source.display_order
WHEN NOT MATCHED THEN 
    INSERT (id, link_key, icon, url, badge, badge_color, display_order, is_active)
    VALUES (source.id, source.link_key, source.icon, source.url, source.badge, source.badge_color, source.display_order, source.is_active);
GO

-- Profile Nav Link Translations
MERGE INTO x_profile_nav_link_locales AS target
USING (VALUES 
    (CAST('f47ac10b-58cc-4372-a567-0e02b2c3d479' AS UNIQUEIDENTIFIER), N'en-US', N'My Profile'),
    (CAST('e36ab09a-47bb-4261-9456-fdf1a1b2c368' AS UNIQUEIDENTIFIER), N'en-US', N'Security & Credentials'),
    (CAST('d25ea989-36aa-4150-8345-ece090a1b257' AS UNIQUEIDENTIFIER), N'en-US', N'Language & Preferences'),

    (CAST('f47ac10b-58cc-4372-a567-0e02b2c3d479' AS UNIQUEIDENTIFIER), N'es-ES', N'Mi Perfil'),
    (CAST('e36ab09a-47bb-4261-9456-fdf1a1b2c368' AS UNIQUEIDENTIFIER), N'es-ES', N'Seguridad y Credenciales'),
    (CAST('d25ea989-36aa-4150-8345-ece090a1b257' AS UNIQUEIDENTIFIER), N'es-ES', N'Idioma y Preferencias')
) AS source (nav_id, locale_code, label)
ON target.nav_id = source.nav_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET label = source.label
WHEN NOT MATCHED THEN 
    INSERT (nav_id, locale_code, label) VALUES (source.nav_id, source.locale_code, source.label);
GO

-- 4. Seed Base Pages & Locales
MERGE INTO x_pages AS target
USING (VALUES 
    (CAST('d9b2e8f1-4c7a-412e-8901-b2c3d4e5f607' AS UNIQUEIDENTIFIER), N'dashboard', N'General')
) AS source (id, page_key, category)
ON target.page_key = source.page_key
WHEN MATCHED THEN 
    UPDATE SET category = source.category
WHEN NOT MATCHED THEN 
    INSERT (id, page_key, category) VALUES (source.id, source.page_key, source.category);
GO

-- Page Translations
MERGE INTO x_page_locales AS target
USING (VALUES 
    (CAST('d9b2e8f1-4c7a-412e-8901-b2c3d4e5f607' AS UNIQUEIDENTIFIER), N'en-US', N'Operational Dashboard', N'Main Workspace & Action Queue'),
    (CAST('d9b2e8f1-4c7a-412e-8901-b2c3d4e5f607' AS UNIQUEIDENTIFIER), N'es-ES', N'Panel Operativo', N'Espacio de Trabajo Principal')
) AS source (page_id, locale_code, title, description)
ON target.page_id = source.page_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET title = source.title, description = source.description
WHEN NOT MATCHED THEN 
    INSERT (page_id, locale_code, title, description) VALUES (source.page_id, source.locale_code, source.title, source.description);
GO

-- 5. Seed Base Mappers & Locales (Data sources: Tables, Views, Sprocs)
MERGE INTO x_mappers AS target
USING (VALUES 
    (CAST('b3f810e2-8924-4d1a-b605-7281f9a1c0d4' AS UNIQUEIDENTIFIER), N'mapper-asset-master', N'TABLE', N'dbo.Assets'),
    (CAST('c4a921f3-9035-4e2b-c716-83920ab2d1e5' AS UNIQUEIDENTIFIER), N'mapper-user-master', N'TABLE', N'dbo.Users')
) AS source (id, mapper_key, source_type, source_name)
ON target.mapper_key = source.mapper_key
WHEN MATCHED THEN 
    UPDATE SET source_type = source.source_type, source_name = source.source_name
WHEN NOT MATCHED THEN 
    INSERT (id, mapper_key, source_type, source_name)
    VALUES (source.id, source.mapper_key, source.source_type, source.source_name);
GO

-- Mapper Translations
MERGE INTO x_mapper_locales AS target
USING (VALUES 
    (CAST('b3f810e2-8924-4d1a-b605-7281f9a1c0d4' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Master Repository Mapper', N'Full schema mapper for enterprise physical & digital assets'),
    (CAST('c4a921f3-9035-4e2b-c716-83920ab2d1e5' AS UNIQUEIDENTIFIER), N'en-US', N'User Profile Master Mapper', N'Full schema mapper for user accounts and department assignments')
) AS source (mapper_id, locale_code, display_name, description)
ON target.mapper_id = source.mapper_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET display_name = source.display_name, description = source.description
WHEN NOT MATCHED THEN 
    INSERT (mapper_id, locale_code, display_name, description)
    VALUES (source.mapper_id, source.locale_code, source.display_name, source.description);
GO

-- 6. Seed Mapper Flavors & Locales (Reduced & Configured Field Subsets)
MERGE INTO x_mapper_flavors AS target
USING (VALUES 
    (CAST('e5c14305-1257-4f4d-9938-a5b4c3d2e1f0' AS UNIQUEIDENTIFIER), N'flavor-asset-registration', CAST('b3f810e2-8924-4d1a-b605-7281f9a1c0d4' AS UNIQUEIDENTIFIER)),
    (CAST('f6d25416-2368-4a5e-8049-b6c5d4e3f201' AS UNIQUEIDENTIFIER), N'flavor-asset-inspection-readonly', CAST('b3f810e2-8924-4d1a-b605-7281f9a1c0d4' AS UNIQUEIDENTIFIER)),
    (CAST('07e36527-3479-4b6f-9150-c7d6e5f40312' AS UNIQUEIDENTIFIER), N'flavor-user-profile', CAST('c4a921f3-9035-4e2b-c716-83920ab2d1e5' AS UNIQUEIDENTIFIER))
) AS source (id, flavor_key, mapper_id)
ON target.flavor_key = source.flavor_key
WHEN MATCHED THEN 
    UPDATE SET mapper_id = source.mapper_id
WHEN NOT MATCHED THEN 
    INSERT (id, flavor_key, mapper_id) VALUES (source.id, source.flavor_key, source.mapper_id);
GO

-- Mapper Flavor Translations
MERGE INTO x_mapper_flavor_locales AS target
USING (VALUES 
    (CAST('e5c14305-1257-4f4d-9938-a5b4c3d2e1f0' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Registration Flavor', N'Editable subset of fields for creating new assets'),
    (CAST('f6d25416-2368-4a5e-8049-b6c5d4e3f201' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Inspection Readonly Flavor', N'Readonly subset of fields for field audit inspection records'),
    (CAST('07e36527-3479-4b6f-9150-c7d6e5f40312' AS UNIQUEIDENTIFIER), N'en-US', N'User Profile Settings Flavor', N'Editable user details and preferences subset')
) AS source (flavor_id, locale_code, display_name, description)
ON target.flavor_id = source.flavor_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET display_name = source.display_name, description = source.description
WHEN NOT MATCHED THEN 
    INSERT (flavor_id, locale_code, display_name, description)
    VALUES (source.flavor_id, source.locale_code, source.display_name, source.description);
GO

-- 7. Seed Base Dynamic Forms & Locales (Includes visible_clause and flavor_id)
MERGE INTO x_forms AS target
USING (VALUES 
    (CAST('9a7b6c5d-4e3f-412a-8901-23456789abcd' AS UNIQUEIDENTIFIER), N'asset-create', CAST('e5c14305-1257-4f4d-9938-a5b4c3d2e1f0' AS UNIQUEIDENTIFIER), N'standard', N'user.isAuthenticated && user.hasPermission("Asset.Create")', 1, N'left', 12, 1),
    (CAST('0b8c7d6e-5f4a-423b-9012-3456789abcde' AS UNIQUEIDENTIFIER), N'user-profile', CAST('07e36527-3479-4b6f-9150-c7d6e5f40312' AS UNIQUEIDENTIFIER), N'standard', N'user.isAuthenticated', 1, N'left', 12, 1),
    (CAST('1c9d8e7f-6a5b-434c-a123-456789abcdef' AS UNIQUEIDENTIFIER), N'asset-inspection', CAST('f6d25416-2368-4a5e-8049-b6c5d4e3f201' AS UNIQUEIDENTIFIER), N'detail', N'user.isAuthenticated', 0, N'top', 12, 0),
    (CAST('2dae9f80-7b6c-445d-b234-56789abcdef0' AS UNIQUEIDENTIFIER), N'search-asset-filter', NULL, N'search', N'user.isAuthenticated', 1, N'left', 12, 1),
    (CAST('3ebfa091-8c7d-456e-c345-6789abcdef01' AS UNIQUEIDENTIFIER), N'grid-inbox-items', NULL, N'grid', N'user.isAuthenticated && user.inboxCount > 0', 1, N'left', 12, 0)
) AS source (id, form_key, flavor_id, form_type, visible_clause, is_editable, label_position, grid_cols, show_reset_button)
ON target.form_key = source.form_key
WHEN MATCHED THEN 
    UPDATE SET flavor_id = source.flavor_id, form_type = source.form_type, visible_clause = source.visible_clause, is_editable = source.is_editable, label_position = source.label_position
WHEN NOT MATCHED THEN 
    INSERT (id, form_key, flavor_id, form_type, visible_clause, is_editable, label_position, grid_cols, show_reset_button)
    VALUES (source.id, source.form_key, source.flavor_id, source.form_type, source.visible_clause, source.is_editable, source.label_position, source.grid_cols, source.show_reset_button);
GO

-- Form Translations
MERGE INTO x_form_locales AS target
USING (VALUES 
    (CAST('9a7b6c5d-4e3f-412a-8901-23456789abcd' AS UNIQUEIDENTIFIER), N'en-US', N'New Asset Registration Form', N'Asset Registration', N'Enter asset metadata for cataloging in the asset management system.', N'Please ensure asset barcode tag complies with AST-YYYY-NNNN standard.', N'Save Asset'),
    (CAST('0b8c7d6e-5f4a-423b-9012-3456789abcde' AS UNIQUEIDENTIFIER), N'en-US', N'User Profile & Preferences Form', N'User Profile & Preferences', N'Manage personal details, department assignment, and notification toggles.', N'User changes will take effect upon saving.', N'Update Profile'),
    (CAST('1c9d8e7f-6a5b-434c-a123-456789abcdef' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Condition Inspection Audit', N'Asset Condition Inspection Audit (Read-Only Archive)', N'Completed physical asset audit report. In read-only mode, all fields are locked.', N'Record ID: AUD-2026-8810 • Audit Status: Closed & Approved', N'Acknowledge Report'),
    (CAST('2dae9f80-7b6c-445d-b234-56789abcdef0' AS UNIQUEIDENTIFIER), N'en-US', N'Filter Inventory & Audit Records', N'Asset Search & Criteria Filter', N'Filter items across category, warranty level, and critical tags', NULL, N'Apply Filters'),
    (CAST('3ebfa091-8c7d-456e-c345-6789abcdef01' AS UNIQUEIDENTIFIER), N'en-US', N'Priority Tasks Requiring Action', N'Inbox Action Items & Asset Maintenance Queue', N'Recent alerts, inspection assignments, and work orders', NULL, N'Submit')
) AS source (form_id, locale_code, caption, title, description, form_info, submit_button_text)
ON target.form_id = source.form_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET caption = source.caption, title = source.title, description = source.description, form_info = source.form_info, submit_button_text = source.submit_button_text
WHEN NOT MATCHED THEN 
    INSERT (form_id, locale_code, caption, title, description, form_info, submit_button_text)
    VALUES (source.form_id, source.locale_code, source.caption, source.title, source.description, source.form_info, source.submit_button_text);
GO

-- 8. Seed Page-to-Forms Junction Mappings (x_page_forms with page-specific visible_clause)
MERGE INTO x_page_forms AS target
USING (VALUES 
    (CAST('4fcab102-9d8e-467f-d456-789abcdef012' AS UNIQUEIDENTIFIER), CAST('d9b2e8f1-4c7a-412e-8901-b2c3d4e5f607' AS UNIQUEIDENTIFIER), CAST('2dae9f80-7b6c-445d-b234-56789abcdef0' AS UNIQUEIDENTIFIER), N'page.tab == "search"', 1, 1),
    (CAST('5adbc213-0e9f-4780-e567-89abcdef0123' AS UNIQUEIDENTIFIER), CAST('d9b2e8f1-4c7a-412e-8901-b2c3d4e5f607' AS UNIQUEIDENTIFIER), CAST('3ebfa091-8c7d-456e-c345-6789abcdef01' AS UNIQUEIDENTIFIER), N'page.tab == "inbox"', 2, 1),
    (CAST('6becd324-1fa0-4891-f678-9abcdef01234' AS UNIQUEIDENTIFIER), CAST('d9b2e8f1-4c7a-412e-8901-b2c3d4e5f607' AS UNIQUEIDENTIFIER), CAST('1c9d8e7f-6a5b-434c-a123-456789abcdef' AS UNIQUEIDENTIFIER), N'page.selectedAssetId != null', 3, 1)
) AS source (id, page_id, form_id, visible_clause, display_order, is_active)
ON target.page_id = source.page_id AND target.form_id = source.form_id
WHEN MATCHED THEN 
    UPDATE SET visible_clause = source.visible_clause, display_order = source.display_order, is_active = source.is_active
WHEN NOT MATCHED THEN 
    INSERT (id, page_id, form_id, visible_clause, display_order, is_active)
    VALUES (source.id, source.page_id, source.form_id, source.visible_clause, source.display_order, source.is_active);
GO
