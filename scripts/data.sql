-- ============================================================================
-- DATA.SQL
-- Seed Data Script for Master Locales, Navigation, Pages, Mappers, Flavors & Forms
-- Includes Visible Clauses on Forms & Localized Mapper Flavors
-- Rule: All metadata tables are prefixed with 'x_'
-- All primary/foreign keys use UNIQUEIDENTIFIER (deterministic GUID constants for seed data)
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

-- 2. Seed Base Site Navigation Links & Locales
MERGE INTO x_site_nav_links AS target
USING (VALUES 
    (CAST('11111111-1111-1111-1111-111111111101' AS UNIQUEIDENTIFIER), N'nav-dashboard', N'home', N'/dashboard', 4, N'Main', 1, 1),
    (CAST('11111111-1111-1111-1111-111111111102' AS UNIQUEIDENTIFIER), N'nav-assets', N'box', N'/assets', 12, N'Management', 2, 1),
    (CAST('11111111-1111-1111-1111-111111111103' AS UNIQUEIDENTIFIER), N'nav-compliance', N'check-circle', N'/compliance', NULL, N'Management', 3, 1),
    (CAST('11111111-1111-1111-1111-111111111104' AS UNIQUEIDENTIFIER), N'nav-audits', N'file-text', N'/audits', NULL, N'Archive', 4, 1),
    (CAST('11111111-1111-1111-1111-111111111105' AS UNIQUEIDENTIFIER), N'nav-analytics', N'bar-chart', N'/analytics', NULL, N'Archive', 5, 1)
) AS source (id, link_key, icon, route, badge_count, category, display_order, is_active)
ON target.link_key = source.link_key
WHEN MATCHED THEN 
    UPDATE SET icon = source.icon, route = source.route, badge_count = source.badge_count, category = source.category, display_order = source.display_order
WHEN NOT MATCHED THEN 
    INSERT (id, link_key, icon, route, badge_count, category, display_order, is_active)
    VALUES (source.id, source.link_key, source.icon, source.route, source.badge_count, source.category, source.display_order, source.is_active);

-- Site Nav Link Translations
MERGE INTO x_site_nav_link_locales AS target
USING (VALUES 
    (CAST('11111111-1111-1111-1111-111111111101' AS UNIQUEIDENTIFIER), N'en-US', N'Inbox & Dashboard'),
    (CAST('11111111-1111-1111-1111-111111111102' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Operations'),
    (CAST('11111111-1111-1111-1111-111111111103' AS UNIQUEIDENTIFIER), N'en-US', N'Compliance & Safety'),
    (CAST('11111111-1111-1111-1111-111111111104' AS UNIQUEIDENTIFIER), N'en-US', N'Audit Archive'),
    (CAST('11111111-1111-1111-1111-111111111105' AS UNIQUEIDENTIFIER), N'en-US', N'Reports & Analytics'),

    (CAST('11111111-1111-1111-1111-111111111101' AS UNIQUEIDENTIFIER), N'es-ES', N'Bandeja de Entrada y Panel'),
    (CAST('11111111-1111-1111-1111-111111111102' AS UNIQUEIDENTIFIER), N'es-ES', N'Operaciones de Activos'),
    (CAST('11111111-1111-1111-1111-111111111103' AS UNIQUEIDENTIFIER), N'es-ES', N'Cumplimiento y Seguridad'),
    (CAST('11111111-1111-1111-1111-111111111104' AS UNIQUEIDENTIFIER), N'es-ES', N'Archivo de Auditorías'),
    (CAST('11111111-1111-1111-1111-111111111105' AS UNIQUEIDENTIFIER), N'es-ES', N'Informes y Analítica')
) AS source (nav_id, locale_code, label)
ON target.nav_id = source.nav_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET label = source.label
WHEN NOT MATCHED THEN 
    INSERT (nav_id, locale_code, label) VALUES (source.nav_id, source.locale_code, source.label);

-- 3. Seed Base Mappers & Locales (Data sources: Tables, Views, Sprocs)
MERGE INTO x_mappers AS target
USING (VALUES 
    (CAST('66666666-6666-6666-6666-666666666601' AS UNIQUEIDENTIFIER), N'mapper-asset-master', N'TABLE', N'dbo.Assets'),
    (CAST('66666666-6666-6666-6666-666666666602' AS UNIQUEIDENTIFIER), N'mapper-user-master', N'TABLE', N'dbo.Users')
) AS source (id, mapper_key, source_type, source_name)
ON target.mapper_key = source.mapper_key
WHEN MATCHED THEN 
    UPDATE SET source_type = source.source_type, source_name = source.source_name
WHEN NOT MATCHED THEN 
    INSERT (id, mapper_key, source_type, source_name)
    VALUES (source.id, source.mapper_key, source.source_type, source.source_name);

-- Mapper Translations
MERGE INTO x_mapper_locales AS target
USING (VALUES 
    (CAST('66666666-6666-6666-6666-666666666601' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Master Repository Mapper', N'Full schema mapper for enterprise physical & digital assets'),
    (CAST('66666666-6666-6666-6666-666666666602' AS UNIQUEIDENTIFIER), N'en-US', N'User Profile Master Mapper', N'Full schema mapper for user accounts and department assignments')
) AS source (mapper_id, locale_code, display_name, description)
ON target.mapper_id = source.mapper_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET display_name = source.display_name, description = source.description
WHEN NOT MATCHED THEN 
    INSERT (mapper_id, locale_code, display_name, description)
    VALUES (source.mapper_id, source.locale_code, source.display_name, source.description);

-- 4. Seed Mapper Flavors & Locales (Reduced & Configured Field Subsets)
MERGE INTO x_mapper_flavors AS target
USING (VALUES 
    (CAST('77777777-7777-7777-7777-777777777701' AS UNIQUEIDENTIFIER), N'flavor-asset-registration', CAST('66666666-6666-6666-6666-666666666601' AS UNIQUEIDENTIFIER)),
    (CAST('77777777-7777-7777-7777-777777777702' AS UNIQUEIDENTIFIER), N'flavor-asset-inspection-readonly', CAST('66666666-6666-6666-6666-666666666601' AS UNIQUEIDENTIFIER)),
    (CAST('77777777-7777-7777-7777-777777777703' AS UNIQUEIDENTIFIER), N'flavor-user-profile', CAST('66666666-6666-6666-6666-666666666602' AS UNIQUEIDENTIFIER))
) AS source (id, flavor_key, mapper_id)
ON target.flavor_key = source.flavor_key
WHEN MATCHED THEN 
    UPDATE SET mapper_id = source.mapper_id
WHEN NOT MATCHED THEN 
    INSERT (id, flavor_key, mapper_id) VALUES (source.id, source.flavor_key, source.mapper_id);

-- Mapper Flavor Translations
MERGE INTO x_mapper_flavor_locales AS target
USING (VALUES 
    (CAST('77777777-7777-7777-7777-777777777701' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Registration Flavor', N'Editable subset of fields for creating new assets'),
    (CAST('77777777-7777-7777-7777-777777777702' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Inspection Readonly Flavor', N'Readonly subset of fields for field audit inspection records'),
    (CAST('77777777-7777-7777-7777-777777777703' AS UNIQUEIDENTIFIER), N'en-US', N'User Profile Settings Flavor', N'Editable user details and preferences subset')
) AS source (flavor_id, locale_code, display_name, description)
ON target.flavor_id = source.flavor_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET display_name = source.display_name, description = source.description
WHEN NOT MATCHED THEN 
    INSERT (flavor_id, locale_code, display_name, description)
    VALUES (source.flavor_id, source.locale_code, source.display_name, source.description);

-- 5. Seed Base Dynamic Forms & Locales (Includes visible_clause and flavor_id)
MERGE INTO x_forms AS target
USING (VALUES 
    (CAST('44444444-4444-4444-4444-444444444401' AS UNIQUEIDENTIFIER), N'asset-create', CAST('77777777-7777-7777-7777-777777777701' AS UNIQUEIDENTIFIER), N'standard', N'user.isAuthenticated && user.hasPermission("Asset.Create")', 1, N'left', 12, 1),
    (CAST('44444444-4444-4444-4444-444444444402' AS UNIQUEIDENTIFIER), N'user-profile', CAST('77777777-7777-7777-7777-777777777703' AS UNIQUEIDENTIFIER), N'standard', N'user.isAuthenticated', 1, N'left', 12, 1),
    (CAST('44444444-4444-4444-4444-444444444403' AS UNIQUEIDENTIFIER), N'asset-inspection', CAST('77777777-7777-7777-7777-777777777702' AS UNIQUEIDENTIFIER), N'detail', N'user.isAuthenticated', 0, N'top', 12, 0),
    (CAST('44444444-4444-4444-4444-444444444404' AS UNIQUEIDENTIFIER), N'search-asset-filter', NULL, N'search', N'user.isAuthenticated', 1, N'left', 12, 1),
    (CAST('44444444-4444-4444-4444-444444444405' AS UNIQUEIDENTIFIER), N'grid-inbox-items', NULL, N'grid', N'user.isAuthenticated && user.inboxCount > 0', 1, N'left', 12, 0)
) AS source (id, form_key, flavor_id, form_type, visible_clause, is_editable, label_position, grid_cols, show_reset_button)
ON target.form_key = source.form_key
WHEN MATCHED THEN 
    UPDATE SET flavor_id = source.flavor_id, form_type = source.form_type, visible_clause = source.visible_clause, is_editable = source.is_editable, label_position = source.label_position
WHEN NOT MATCHED THEN 
    INSERT (id, form_key, flavor_id, form_type, visible_clause, is_editable, label_position, grid_cols, show_reset_button)
    VALUES (source.id, source.form_key, source.flavor_id, source.form_type, source.visible_clause, source.is_editable, source.label_position, source.grid_cols, source.show_reset_button);

-- Form Translations
MERGE INTO x_form_locales AS target
USING (VALUES 
    (CAST('44444444-4444-4444-4444-444444444401' AS UNIQUEIDENTIFIER), N'en-US', N'New Asset Registration Form', N'Asset Registration', N'Enter asset metadata for cataloging in the asset management system.', N'Please ensure asset barcode tag complies with AST-YYYY-NNNN standard.', N'Save Asset'),
    (CAST('44444444-4444-4444-4444-444444444402' AS UNIQUEIDENTIFIER), N'en-US', N'User Profile & Preferences Form', N'User Profile & Preferences', N'Manage personal details, department assignment, and notification toggles.', N'User changes will take effect upon saving.', N'Update Profile'),
    (CAST('44444444-4444-4444-4444-444444444403' AS UNIQUEIDENTIFIER), N'en-US', N'Asset Condition Inspection Audit', N'Asset Condition Inspection Audit (Read-Only Archive)', N'Completed physical asset audit report. In read-only mode, all fields are locked.', N'Record ID: AUD-2026-8810 • Audit Status: Closed & Approved', N'Acknowledge Report'),
    (CAST('44444444-4444-4444-4444-444444444404' AS UNIQUEIDENTIFIER), N'en-US', N'Filter Inventory & Audit Records', N'Asset Search & Criteria Filter', N'Filter items across category, warranty level, and critical tags', NULL, N'Apply Filters'),
    (CAST('44444444-4444-4444-4444-444444444405' AS UNIQUEIDENTIFIER), N'en-US', N'Priority Tasks Requiring Action', N'Inbox Action Items & Asset Maintenance Queue', N'Recent alerts, inspection assignments, and work orders', NULL, N'Submit')
) AS source (form_id, locale_code, caption, title, description, form_info, submit_button_text)
ON target.form_id = source.form_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN 
    UPDATE SET caption = source.caption, title = source.title, description = source.description, form_info = source.form_info, submit_button_text = source.submit_button_text
WHEN NOT MATCHED THEN 
    INSERT (form_id, locale_code, caption, title, description, form_info, submit_button_text)
    VALUES (source.form_id, source.locale_code, source.caption, source.title, source.description, source.form_info, source.submit_button_text);

-- 6. Seed Page-to-Forms Junction Mappings (x_page_forms with page-specific visible_clause)
MERGE INTO x_page_forms AS target
USING (VALUES 
    (CAST('55555555-5555-5555-5555-555555555501' AS UNIQUEIDENTIFIER), CAST('33333333-3333-3333-3333-333333333301' AS UNIQUEIDENTIFIER), CAST('44444444-4444-4444-4444-444444444404' AS UNIQUEIDENTIFIER), N'page.tab == "search"', 1, 1),
    (CAST('55555555-5555-5555-5555-555555555502' AS UNIQUEIDENTIFIER), CAST('33333333-3333-3333-3333-333333333301' AS UNIQUEIDENTIFIER), CAST('44444444-4444-4444-4444-444444444405' AS UNIQUEIDENTIFIER), N'page.tab == "inbox"', 2, 1),
    (CAST('55555555-5555-5555-5555-555555555503' AS UNIQUEIDENTIFIER), CAST('33333333-3333-3333-3333-333333333301' AS UNIQUEIDENTIFIER), CAST('44444444-4444-4444-4444-444444444403' AS UNIQUEIDENTIFIER), N'page.selectedAssetId != null', 3, 1)
) AS source (id, page_id, form_id, visible_clause, display_order, is_active)
ON target.page_id = source.page_id AND target.form_id = source.form_id
WHEN MATCHED THEN 
    UPDATE SET visible_clause = source.visible_clause, display_order = source.display_order, is_active = source.is_active
WHEN NOT MATCHED THEN 
    INSERT (id, page_id, form_id, visible_clause, display_order, is_active)
    VALUES (source.id, source.page_id, source.form_id, source.visible_clause, source.display_order, source.is_active);
