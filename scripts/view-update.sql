-- ============================================================================
-- VIEW-UPDATE.SQL
-- Localized T-SQL Database Views for Metadata Entities (vw_x_*)
-- Includes Mappers, Mapper Flavors, Visible Clauses, and en-US fallback
-- ============================================================================

-- 1. Supported Locales Master View
CREATE OR ALTER VIEW vw_x_supported_locales AS
SELECT 
    locale_code,
    display_name,
    is_default,
    is_active
FROM x_locales
WHERE is_active = 1;
GO

-- 2. Localized Site Navigation Links Metadata View
CREATE OR ALTER VIEW vw_x_site_nav_links_localized AS
SELECT 
    sn.id AS nav_id,
    sn.link_key,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.label, def_loc.label, N'Unlabeled') AS label,
    sn.icon,
    sn.route,
    sn.badge_count,
    sn.category,
    sn.display_order
FROM x_site_nav_links sn
CROSS JOIN x_locales l
LEFT JOIN x_site_nav_link_locales req_loc 
    ON sn.id = req_loc.nav_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_site_nav_link_locales def_loc 
    ON sn.id = def_loc.nav_id AND def_loc.locale_code = N'en-US'
WHERE sn.is_active = 1 AND l.is_active = 1;
GO

-- 3. Localized Profile Navigation Links Metadata View
CREATE OR ALTER VIEW vw_x_profile_nav_links_localized AS
SELECT 
    pn.id AS nav_id,
    pn.link_key,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.label, def_loc.label, N'Unlabeled') AS label,
    pn.icon,
    pn.url,
    pn.badge,
    pn.badge_color,
    pn.display_order
FROM x_profile_nav_links pn
CROSS JOIN x_locales l
LEFT JOIN x_profile_nav_link_locales req_loc 
    ON pn.id = req_loc.nav_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_profile_nav_link_locales def_loc 
    ON pn.id = def_loc.nav_id AND def_loc.locale_code = N'en-US'
WHERE pn.is_active = 1 AND l.is_active = 1;
GO

-- 4. Localized Pages Metadata View
CREATE OR ALTER VIEW vw_x_pages_localized AS
SELECT 
    p.id AS page_guid,
    p.page_key,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.title, def_loc.title, p.page_key) AS title,
    COALESCE(req_loc.description, def_loc.description) AS description,
    p.category,
    p.created_at
FROM x_pages p
CROSS JOIN x_locales l
LEFT JOIN x_page_locales req_loc 
    ON p.id = req_loc.page_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_page_locales def_loc 
    ON p.id = def_loc.page_id AND def_loc.locale_code = N'en-US'
WHERE l.is_active = 1;
GO

-- 5. Localized Mappers Metadata View
CREATE OR ALTER VIEW vw_x_mappers_localized AS
SELECT 
    m.id AS mapper_guid,
    m.mapper_key,
    m.source_type,
    m.source_name,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.display_name, def_loc.display_name, m.mapper_key) AS display_name,
    COALESCE(req_loc.description, def_loc.description) AS description,
    m.created_at
FROM x_mappers m
CROSS JOIN x_locales l
LEFT JOIN x_mapper_locales req_loc 
    ON m.id = req_loc.mapper_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_mapper_locales def_loc 
    ON m.id = def_loc.mapper_id AND def_loc.locale_code = N'en-US'
WHERE l.is_active = 1;
GO

-- 6. Localized Mapper Flavors Metadata View
CREATE OR ALTER VIEW vw_x_mapper_flavors_localized AS
SELECT 
    mf.id AS flavor_guid,
    mf.flavor_key,
    mf.mapper_id,
    m.mapper_key,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.display_name, def_loc.display_name, mf.flavor_key) AS display_name,
    COALESCE(req_loc.description, def_loc.description) AS description,
    mf.created_at
FROM x_mapper_flavors mf
JOIN x_mappers m ON mf.mapper_id = m.id
CROSS JOIN x_locales l
LEFT JOIN x_mapper_flavor_locales req_loc 
    ON mf.id = req_loc.flavor_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_mapper_flavor_locales def_loc 
    ON mf.id = def_loc.flavor_id AND def_loc.locale_code = N'en-US'
WHERE l.is_active = 1;
GO

-- 7. Localized Mapper Flavor Fields Metadata View (Customized field subset per flavor)
CREATE OR ALTER VIEW vw_x_mapper_flavor_fields_localized AS
SELECT 
    mff.id AS flavor_field_guid,
    mff.flavor_id,
    mf.flavor_key,
    mff.mapper_field_id,
    mp.field_name AS mapper_field_name,
    mff.key_name,
    mff.field_type,
    mff.is_editable,
    mff.is_readonly,
    mff.is_disabled,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.label, def_loc.label, mff.key_name) AS label,
    COALESCE(req_loc.placeholder, def_loc.placeholder) AS placeholder,
    COALESCE(req_loc.default_value, def_loc.default_value) AS default_value,
    COALESCE(req_loc.help_text, def_loc.help_text) AS help_text,
    mff.display_order,
    mff.grid_cols,
    mff.custom_css_class
FROM x_mapper_flavor_fields mff
JOIN x_mapper_flavors mf ON mff.flavor_id = mf.id
LEFT JOIN x_mapper_fields mp ON mff.mapper_field_id = mp.id
CROSS JOIN x_locales l
LEFT JOIN x_mapper_flavor_field_locales req_loc 
    ON mff.id = req_loc.field_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_mapper_flavor_field_locales def_loc 
    ON mff.id = def_loc.field_id AND def_loc.locale_code = N'en-US'
WHERE l.is_active = 1;
GO

-- 8. Localized Dynamic Forms Metadata View (Includes visible_clause and flavor references)
CREATE OR ALTER VIEW vw_x_forms_localized AS
SELECT 
    f.id AS form_guid,
    f.form_key,
    f.flavor_id,
    mf.flavor_key,
    vmf.display_name AS flavor_display_name,
    f.form_type,
    f.visible_clause,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.caption, def_loc.caption, f.form_key) AS caption,
    COALESCE(req_loc.title, def_loc.title, f.form_key) AS title,
    COALESCE(req_loc.description, def_loc.description) AS description,
    COALESCE(req_loc.form_info, def_loc.form_info) AS form_info,
    COALESCE(req_loc.submit_button_text, def_loc.submit_button_text, N'Submit') AS submit_button_text,
    f.is_editable,
    f.label_position,
    f.grid_cols,
    f.show_reset_button
FROM x_forms f
LEFT JOIN x_mapper_flavors mf ON f.flavor_id = mf.id
CROSS JOIN x_locales l
LEFT JOIN vw_x_mapper_flavors_localized vmf ON mf.id = vmf.flavor_guid AND vmf.requested_locale = l.locale_code
LEFT JOIN x_form_locales req_loc 
    ON f.id = req_loc.form_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_form_locales def_loc 
    ON f.id = def_loc.form_id AND def_loc.locale_code = N'en-US'
WHERE l.is_active = 1;
GO

-- 9. Localized Page-to-Forms Junction View (Includes page-specific visible_clause)
CREATE OR ALTER VIEW vw_x_page_forms_localized AS
SELECT 
    pf.id AS page_form_guid,
    pf.page_id,
    p.page_key,
    vp.title AS page_title,
    pf.form_id,
    f.form_key,
    vf.caption AS form_caption,
    vf.title AS form_title,
    vf.form_type,
    COALESCE(pf.visible_clause, vf.visible_clause) AS effective_visible_clause,
    vf.is_editable,
    vf.label_position,
    vf.grid_cols,
    l.locale_code AS requested_locale,
    pf.display_order,
    pf.is_active
FROM x_page_forms pf
JOIN x_pages p ON pf.page_id = p.id
JOIN x_forms f ON pf.form_id = f.id
CROSS JOIN x_locales l
LEFT JOIN vw_x_pages_localized vp 
    ON p.id = vp.page_guid AND vp.requested_locale = l.locale_code
LEFT JOIN vw_x_forms_localized vf 
    ON f.id = vf.form_guid AND vf.requested_locale = l.locale_code
WHERE pf.is_active = 1 AND l.is_active = 1;
GO

-- 10. Localized Form Fields Metadata View
CREATE OR ALTER VIEW vw_x_form_fields_localized AS
SELECT 
    ff.id AS field_guid,
    ff.form_id,
    f.form_key,
    ff.key_name,
    ff.field_type,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.label, def_loc.label, ff.key_name) AS label,
    COALESCE(req_loc.placeholder, def_loc.placeholder) AS placeholder,
    COALESCE(req_loc.default_value, def_loc.default_value) AS default_value,
    COALESCE(req_loc.help_text, def_loc.help_text) AS help_text,
    ff.display_order,
    ff.grid_cols,
    ff.custom_css_class,
    ff.is_disabled,
    ff.is_readonly
FROM x_form_fields ff
JOIN x_forms f ON ff.form_id = f.id
CROSS JOIN x_locales l
LEFT JOIN x_form_field_locales req_loc 
    ON ff.id = req_loc.field_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_form_field_locales def_loc 
    ON ff.id = def_loc.field_id AND def_loc.locale_code = N'en-US'
WHERE l.is_active = 1;
GO

-- 11. Localized Form Field Validators Metadata View
CREATE OR ALTER VIEW vw_x_form_field_validators_localized AS
SELECT 
    v.id AS validator_guid,
    v.field_id,
    v.validator_type,
    v.validator_value,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.message, def_loc.message, N'Validation error') AS message
FROM x_form_field_validators v
CROSS JOIN x_locales l
LEFT JOIN x_form_field_validator_locales req_loc 
    ON v.id = req_loc.validator_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_form_field_validator_locales def_loc 
    ON v.id = def_loc.validator_id AND def_loc.locale_code = N'en-US'
WHERE l.is_active = 1;
GO

-- 12. Localized Form Field Options Metadata View
CREATE OR ALTER VIEW vw_x_form_field_options_localized AS
SELECT 
    o.id AS option_guid,
    o.field_id,
    o.option_value,
    l.locale_code AS requested_locale,
    COALESCE(req_loc.option_label, def_loc.option_label, o.option_value) AS option_label,
    o.is_disabled,
    o.display_order
FROM x_form_field_options o
CROSS JOIN x_locales l
LEFT JOIN x_form_field_option_locales req_loc 
    ON o.id = req_loc.option_id AND req_loc.locale_code = l.locale_code
LEFT JOIN x_form_field_option_locales def_loc 
    ON o.id = def_loc.option_id AND def_loc.locale_code = N'en-US'
WHERE l.is_active = 1;
GO
