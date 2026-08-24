-- ============================================================================
-- SCHEMA-UPDATE.SQL
-- Localized T-SQL Schema Script for Metadata & Dynamic Forms Architecture
-- Rules:
-- 1. ALL metadata entities start with the prefix 'x_'
-- 2. ALL primary/foreign keys use UNIQUEIDENTIFIER (DEFAULT NEWID())
-- 3. ALL string types use NVARCHAR
-- 4. Many-to-many relationship between pages and forms via 'x_page_forms'
-- 5. Visible clauses on forms and page-forms ('visible_clause')
-- 6. Mappers ('x_mappers') and Mapper Flavors ('x_mapper_flavors') with localization
-- 7. ALL tables include mandatory audit columns: date_created, created_by_id, date_updated, updated_by_id
-- ============================================================================

-- 1. Master Supported Locales Metadata Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_locales')
BEGIN
    CREATE TABLE x_locales (
        locale_code NVARCHAR(10) NOT NULL PRIMARY KEY,
        display_name NVARCHAR(64) NOT NULL,
        is_default BIT NOT NULL DEFAULT 0,
        is_active BIT NOT NULL DEFAULT 1,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END;

-- 2. Site Navigation Links Base Metadata Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_site_nav_links')
BEGIN
    CREATE TABLE x_site_nav_links (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        link_key NVARCHAR(64) NOT NULL UNIQUE,
        icon NVARCHAR(64) NOT NULL,
        route NVARCHAR(256) NOT NULL,
        badge_count INT NULL,
        category NVARCHAR(64) NOT NULL DEFAULT N'Main',
        display_order INT NOT NULL DEFAULT 0,
        is_active BIT NOT NULL DEFAULT 1,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_site_nav_link_locales')
BEGIN
    CREATE TABLE x_site_nav_link_locales (
        nav_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_site_nav_links(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        label NVARCHAR(128) NOT NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_site_nav_link_locales PRIMARY KEY (nav_id, locale_code)
    );
END;

-- 3. Profile Navigation Links Base Metadata Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_profile_nav_links')
BEGIN
    CREATE TABLE x_profile_nav_links (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        link_key NVARCHAR(64) NOT NULL UNIQUE,
        icon NVARCHAR(64) NOT NULL,
        url NVARCHAR(256) NOT NULL,
        badge NVARCHAR(32) NULL,
        badge_color NVARCHAR(32) NULL,
        display_order INT NOT NULL DEFAULT 0,
        is_active BIT NOT NULL DEFAULT 1,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_profile_nav_link_locales')
BEGIN
    CREATE TABLE x_profile_nav_link_locales (
        nav_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_profile_nav_links(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        label NVARCHAR(128) NOT NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_profile_nav_link_locales PRIMARY KEY (nav_id, locale_code)
    );
END;

-- 4. Pages Base Metadata Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_pages')
BEGIN
    CREATE TABLE x_pages (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        page_key NVARCHAR(64) NOT NULL UNIQUE,
        category NVARCHAR(64) NOT NULL DEFAULT N'General',
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_page_locales')
BEGIN
    CREATE TABLE x_page_locales (
        page_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_pages(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        title NVARCHAR(128) NOT NULL,
        description NVARCHAR(MAX) NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_page_locales PRIMARY KEY (page_id, locale_code)
    );
END;

-- 5. Mappers Base Metadata Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_mappers')
BEGIN
    CREATE TABLE x_mappers (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        mapper_key NVARCHAR(64) NOT NULL UNIQUE,
        source_type NVARCHAR(32) NOT NULL DEFAULT N'TABLE',
        source_name NVARCHAR(128) NOT NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_mapper_locales')
BEGIN
    CREATE TABLE x_mapper_locales (
        mapper_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_mappers(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        display_name NVARCHAR(128) NOT NULL,
        description NVARCHAR(MAX) NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_mapper_locales PRIMARY KEY (mapper_id, locale_code)
    );
END;

-- 6. Mapper Fields Base Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_mapper_fields')
BEGIN
    CREATE TABLE x_mapper_fields (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        mapper_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_mappers(id) ON DELETE CASCADE,
        field_name NVARCHAR(64) NOT NULL,
        data_type NVARCHAR(32) NOT NULL DEFAULT N'NVARCHAR',
        is_nullable BIT NOT NULL DEFAULT 1,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT UQ_x_mapper_field_name UNIQUE (mapper_id, field_name)
    );
END;

-- 7. Mapper Flavors Base Metadata Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_mapper_flavors')
BEGIN
    CREATE TABLE x_mapper_flavors (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        flavor_key NVARCHAR(64) NOT NULL,
        mapper_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_mappers(id) ON DELETE CASCADE,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT UQ_x_mapper_flavors_mapper_flavor UNIQUE (mapper_id, flavor_key)
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_mapper_flavor_locales')
BEGIN
    CREATE TABLE x_mapper_flavor_locales (
        flavor_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_mapper_flavors(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        display_name NVARCHAR(128) NOT NULL,
        description NVARCHAR(MAX) NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_mapper_flavor_locales PRIMARY KEY (flavor_id, locale_code)
    );
END;

-- 8. Mapper Flavor Fields Base Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_mapper_flavor_fields')
BEGIN
    CREATE TABLE x_mapper_flavor_fields (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        flavor_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_mapper_flavors(id) ON DELETE CASCADE,
        mapper_field_id UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES x_mapper_fields(id) ON DELETE NO ACTION,
        key_name NVARCHAR(64) NOT NULL,
        field_type NVARCHAR(32) NOT NULL DEFAULT N'text',
        is_editable BIT NOT NULL DEFAULT 1,
        is_readonly BIT NOT NULL DEFAULT 0,
        is_disabled BIT NOT NULL DEFAULT 0,
        display_order INT NOT NULL DEFAULT 0,
        grid_cols INT NOT NULL DEFAULT 12,
        custom_css_class NVARCHAR(128) NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT UQ_x_mapper_flavor_field_key UNIQUE (flavor_id, key_name)
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_mapper_flavor_field_locales')
BEGIN
    CREATE TABLE x_mapper_flavor_field_locales (
        field_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_mapper_flavor_fields(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        label NVARCHAR(128) NOT NULL,
        placeholder NVARCHAR(256) NULL,
        default_value NVARCHAR(MAX) NULL,
        help_text NVARCHAR(MAX) NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_mapper_flavor_field_locales PRIMARY KEY (field_id, locale_code)
    );
END;

-- 9. Dynamic Forms Base Metadata Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_forms')
BEGIN
    CREATE TABLE x_forms (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        form_key NVARCHAR(64) NOT NULL UNIQUE,
        flavor_id UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES x_mapper_flavors(id) ON DELETE SET NULL,
        form_type NVARCHAR(32) NOT NULL DEFAULT N'standard',
        visible_clause NVARCHAR(MAX) NULL,
        is_editable BIT NOT NULL DEFAULT 1,
        label_position NVARCHAR(16) NOT NULL DEFAULT N'left',
        grid_cols INT NOT NULL DEFAULT 12,
        show_reset_button BIT NOT NULL DEFAULT 1,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_form_locales')
BEGIN
    CREATE TABLE x_form_locales (
        form_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_forms(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        caption NVARCHAR(256) NOT NULL,
        title NVARCHAR(256) NOT NULL,
        description NVARCHAR(MAX) NULL,
        form_info NVARCHAR(MAX) NULL,
        submit_button_text NVARCHAR(64) NOT NULL DEFAULT N'Submit',
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_form_locales PRIMARY KEY (form_id, locale_code)
    );
END;

-- 10. Page to Forms Junction Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_page_forms')
BEGIN
    CREATE TABLE x_page_forms (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        page_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_pages(id) ON DELETE CASCADE,
        form_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_forms(id) ON DELETE CASCADE,
        visible_clause NVARCHAR(MAX) NULL,
        display_order INT NOT NULL DEFAULT 0,
        is_active BIT NOT NULL DEFAULT 1,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT UQ_x_page_forms UNIQUE (page_id, form_id)
    );
END;

-- 11. Form Fields Base Metadata Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_form_fields')
BEGIN
    CREATE TABLE x_form_fields (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        form_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_forms(id) ON DELETE CASCADE,
        key_name NVARCHAR(64) NOT NULL,
        field_type NVARCHAR(32) NOT NULL DEFAULT N'text',
        display_order INT NOT NULL DEFAULT 0,
        grid_cols INT NOT NULL DEFAULT 12,
        custom_css_class NVARCHAR(128) NULL,
        is_disabled BIT NOT NULL DEFAULT 0,
        is_readonly BIT NOT NULL DEFAULT 0,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT UQ_x_form_field_key UNIQUE (form_id, key_name)
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_form_field_locales')
BEGIN
    CREATE TABLE x_form_field_locales (
        field_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_form_fields(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        label NVARCHAR(128) NOT NULL,
        placeholder NVARCHAR(256) NULL,
        default_value NVARCHAR(MAX) NULL,
        help_text NVARCHAR(MAX) NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_form_field_locales PRIMARY KEY (field_id, locale_code)
    );
END;

-- 12. Form Field Validators Base Metadata Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_form_field_validators')
BEGIN
    CREATE TABLE x_form_field_validators (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        field_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_form_fields(id) ON DELETE CASCADE,
        validator_type NVARCHAR(32) NOT NULL,
        validator_value NVARCHAR(256) NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_form_field_validator_locales')
BEGIN
    CREATE TABLE x_form_field_validator_locales (
        validator_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_form_field_validators(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        message NVARCHAR(256) NOT NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_form_field_validator_locales PRIMARY KEY (validator_id, locale_code)
    );
END;

-- 13. Form Field Options Base Metadata Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_form_field_options')
BEGIN
    CREATE TABLE x_form_field_options (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        field_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_form_fields(id) ON DELETE CASCADE,
        option_value NVARCHAR(128) NOT NULL,
        is_disabled BIT NOT NULL DEFAULT 0,
        display_order INT NOT NULL DEFAULT 0,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_form_field_option_locales')
BEGIN
    CREATE TABLE x_form_field_option_locales (
        option_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_form_field_options(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        option_label NVARCHAR(128) NOT NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_form_field_option_locales PRIMARY KEY (option_id, locale_code)
    );
END;
GO

-- 14. Master Users Table for Authentication & User Info
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'users')
BEGIN
    CREATE TABLE users (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        username NVARCHAR(64) NOT NULL UNIQUE,
        first_name NVARCHAR(64) NOT NULL,
        last_name NVARCHAR(64) NOT NULL,
        email NVARCHAR(256) NOT NULL UNIQUE,
        password_hash NVARCHAR(512) NOT NULL,
        role INT NOT NULL DEFAULT 4,
        provider NVARCHAR(32) NOT NULL DEFAULT N'local',
        avatar_url NVARCHAR(512) NULL,
        preferred_language NVARCHAR(10) NOT NULL DEFAULT N'en-US',
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END
ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('users') AND name = 'role' AND system_type_id = TYPE_ID('nvarchar'))
BEGIN
    DECLARE @ConstraintName NVARCHAR(256);
    SELECT @ConstraintName = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'users') AND c.name = N'role';

    IF @ConstraintName IS NOT NULL
    BEGIN
        EXEC(N'ALTER TABLE users DROP CONSTRAINT [' + @ConstraintName + N']');
    END;

    UPDATE users SET role = N'1' WHERE role = N'admin';
    UPDATE users SET role = N'2' WHERE role = N'manager';
    UPDATE users SET role = N'3' WHERE role IN (N'compliance', N'compliance officer');
    UPDATE users SET role = N'4' WHERE role = N'user' OR TRY_CAST(role AS INT) IS NULL;

    ALTER TABLE users ALTER COLUMN role INT NOT NULL;
    ALTER TABLE users ADD CONSTRAINT DF_users_role DEFAULT 4 FOR role;
END;
GO

-- 15. Pick Lists Master Definition Table, Pick List Items Table & Locales Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_pick_lists')
BEGIN
    CREATE TABLE x_pick_lists (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        list_name NVARCHAR(64) NOT NULL UNIQUE,
        sql_current_values NVARCHAR(MAX) NOT NULL,
        sql_all_values NVARCHAR(MAX) NOT NULL,
        sql_single_value NVARCHAR(MAX) NOT NULL,
        description NVARCHAR(MAX) NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_pick_list_items')
BEGIN
    CREATE TABLE x_pick_list_items (
        id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        list_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_pick_lists(id) ON DELETE CASCADE,
        item_value INT NOT NULL,
        default_display_value NVARCHAR(128) NOT NULL,
        display_order INT NOT NULL DEFAULT 0,
        is_active BIT NOT NULL DEFAULT 1,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT UQ_x_pick_list_items_list_value UNIQUE (list_id, item_value)
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'x_pick_list_item_locales')
BEGIN
    CREATE TABLE x_pick_list_item_locales (
        item_id UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES x_pick_list_items(id) ON DELETE CASCADE,
        locale_code NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES x_locales(locale_code) ON DELETE CASCADE,
        display_value NVARCHAR(128) NOT NULL,
        date_created DATETIME NOT NULL DEFAULT GETUTCDATE(),
        created_by_id UNIQUEIDENTIFIER NULL,
        date_updated DATETIME NULL,
        updated_by_id UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_x_pick_list_item_locales PRIMARY KEY (item_id, locale_code)
    );
END;
GO

-- Dynamic Migration Block: Ensure ALL tables have mandatory audit columns
DECLARE @CurrentTableName NVARCHAR(256);
DECLARE TableAuditCursor CURSOR FOR SELECT name FROM sys.tables;

OPEN TableAuditCursor;
FETCH NEXT FROM TableAuditCursor INTO @CurrentTableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@CurrentTableName) AND name = 'date_created')
    BEGIN
        EXEC(N'ALTER TABLE [' + @CurrentTableName + N'] ADD date_created DATETIME NOT NULL DEFAULT GETUTCDATE()');
    END;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@CurrentTableName) AND name = 'created_by_id')
    BEGIN
        EXEC(N'ALTER TABLE [' + @CurrentTableName + N'] ADD created_by_id UNIQUEIDENTIFIER NULL');
    END;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@CurrentTableName) AND name = 'date_updated')
    BEGIN
        EXEC(N'ALTER TABLE [' + @CurrentTableName + N'] ADD date_updated DATETIME NULL');
    END;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@CurrentTableName) AND name = 'updated_by_id')
    BEGIN
        EXEC(N'ALTER TABLE [' + @CurrentTableName + N'] ADD updated_by_id UNIQUEIDENTIFIER NULL');
    END;

    FETCH NEXT FROM TableAuditCursor INTO @CurrentTableName;
END;

CLOSE TableAuditCursor;
DEALLOCATE TableAuditCursor;
GO
