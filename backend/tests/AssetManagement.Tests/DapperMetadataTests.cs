using System.Data;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Xunit;

namespace AssetManagement.Tests
{
    public class DapperMetadataTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DapperMetadataRepository _repository;

        public DapperMetadataTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            SqliteConnectionFactory factory = new SqliteConnectionFactory(_connection);
            _repository = new DapperMetadataRepository(factory);

            InitializeTestDatabase();
        }

        private void InitializeTestDatabase()
        {
            using SqliteCommand cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE x_locales (
                    locale_code TEXT PRIMARY KEY,
                    display_name TEXT NOT NULL,
                    is_default INTEGER NOT NULL DEFAULT 0,
                    is_active INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE x_site_nav_links (
                    id TEXT PRIMARY KEY,
                    link_key TEXT NOT NULL UNIQUE,
                    icon TEXT NOT NULL,
                    route TEXT NOT NULL,
                    badge_count INTEGER NULL,
                    category TEXT NOT NULL DEFAULT 'Main',
                    display_order INTEGER NOT NULL DEFAULT 0,
                    is_active INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE x_site_nav_link_locales (
                    nav_id TEXT NOT NULL,
                    locale_code TEXT NOT NULL,
                    label TEXT NOT NULL,
                    PRIMARY KEY (nav_id, locale_code)
                );

                CREATE TABLE x_profile_nav_links (
                    id TEXT PRIMARY KEY,
                    link_key TEXT NOT NULL UNIQUE,
                    icon TEXT NOT NULL,
                    url TEXT NOT NULL,
                    badge TEXT NULL,
                    badge_color TEXT NULL,
                    display_order INTEGER NOT NULL DEFAULT 0,
                    is_active INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE x_profile_nav_link_locales (
                    nav_id TEXT NOT NULL,
                    locale_code TEXT NOT NULL,
                    label TEXT NOT NULL,
                    PRIMARY KEY (nav_id, locale_code)
                );

                CREATE TABLE x_pages (
                    id TEXT PRIMARY KEY,
                    page_key TEXT NOT NULL UNIQUE,
                    category TEXT NOT NULL DEFAULT 'General'
                );

                CREATE TABLE x_page_locales (
                    page_id TEXT NOT NULL,
                    locale_code TEXT NOT NULL,
                    title TEXT NOT NULL,
                    description TEXT NULL,
                    PRIMARY KEY (page_id, locale_code)
                );

                CREATE TABLE x_mappers (
                    id TEXT PRIMARY KEY,
                    mapper_key TEXT NOT NULL UNIQUE,
                    source_type TEXT NOT NULL DEFAULT 'TABLE',
                    source_name TEXT NOT NULL
                );

                CREATE TABLE x_mapper_locales (
                    mapper_id TEXT NOT NULL,
                    locale_code TEXT NOT NULL,
                    display_name TEXT NOT NULL,
                    description TEXT NULL,
                    PRIMARY KEY (mapper_id, locale_code)
                );

                CREATE TABLE x_mapper_flavors (
                    id TEXT PRIMARY KEY,
                    flavor_key TEXT NOT NULL UNIQUE,
                    mapper_id TEXT NOT NULL
                );

                CREATE TABLE x_mapper_flavor_locales (
                    flavor_id TEXT NOT NULL,
                    locale_code TEXT NOT NULL,
                    display_name TEXT NOT NULL,
                    description TEXT NULL,
                    PRIMARY KEY (flavor_id, locale_code)
                );

                CREATE TABLE x_mapper_flavor_fields (
                    id TEXT PRIMARY KEY,
                    flavor_id TEXT NOT NULL,
                    mapper_field_id TEXT NULL,
                    key_name TEXT NOT NULL,
                    field_type TEXT NOT NULL DEFAULT 'text',
                    is_editable INTEGER NOT NULL DEFAULT 1,
                    is_readonly INTEGER NOT NULL DEFAULT 0,
                    is_disabled INTEGER NOT NULL DEFAULT 0,
                    display_order INTEGER NOT NULL DEFAULT 0,
                    grid_cols INTEGER NOT NULL DEFAULT 12,
                    custom_css_class TEXT NULL
                );

                CREATE TABLE x_mapper_flavor_field_locales (
                    field_id TEXT NOT NULL,
                    locale_code TEXT NOT NULL,
                    label TEXT NOT NULL,
                    placeholder TEXT NULL,
                    default_value TEXT NULL,
                    help_text TEXT NULL,
                    PRIMARY KEY (field_id, locale_code)
                );

                CREATE TABLE x_forms (
                    id TEXT PRIMARY KEY,
                    form_key TEXT NOT NULL UNIQUE,
                    flavor_id TEXT NULL,
                    form_type TEXT NOT NULL DEFAULT 'standard',
                    visible_clause TEXT NULL,
                    is_editable INTEGER NOT NULL DEFAULT 1,
                    label_position TEXT NOT NULL DEFAULT 'left',
                    grid_cols INTEGER NOT NULL DEFAULT 12,
                    show_reset_button INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE x_form_locales (
                    form_id TEXT NOT NULL,
                    locale_code TEXT NOT NULL,
                    caption TEXT NOT NULL,
                    title TEXT NOT NULL,
                    description TEXT NULL,
                    form_info TEXT NULL,
                    submit_button_text TEXT NOT NULL DEFAULT 'Submit',
                    PRIMARY KEY (form_id, locale_code)
                );

                CREATE TABLE x_page_forms (
                    id TEXT PRIMARY KEY,
                    page_id TEXT NOT NULL,
                    form_id TEXT NOT NULL,
                    visible_clause TEXT NULL,
                    display_order INTEGER NOT NULL DEFAULT 0,
                    is_active INTEGER NOT NULL DEFAULT 1
                );

                CREATE VIEW vw_x_site_nav_links_localized AS
                SELECT 
                    sn.id AS nav_id,
                    sn.link_key,
                    l.locale_code AS requested_locale,
                    COALESCE(req_loc.label, def_loc.label, 'Unlabeled') AS label,
                    sn.icon,
                    sn.route,
                    sn.badge_count,
                    sn.category,
                    sn.display_order
                FROM x_site_nav_links sn
                CROSS JOIN x_locales l
                LEFT JOIN x_site_nav_link_locales req_loc ON sn.id = req_loc.nav_id AND req_loc.locale_code = l.locale_code
                LEFT JOIN x_site_nav_link_locales def_loc ON sn.id = def_loc.nav_id AND def_loc.locale_code = 'en-US'
                WHERE sn.is_active = 1 AND l.is_active = 1;

                CREATE VIEW vw_x_profile_nav_links_localized AS
                SELECT 
                    pn.id AS nav_id,
                    pn.link_key,
                    l.locale_code AS requested_locale,
                    COALESCE(req_loc.label, def_loc.label, 'Unlabeled') AS label,
                    pn.icon,
                    pn.url,
                    pn.badge,
                    pn.badge_color,
                    pn.display_order
                FROM x_profile_nav_links pn
                CROSS JOIN x_locales l
                LEFT JOIN x_profile_nav_link_locales req_loc ON pn.id = req_loc.nav_id AND req_loc.locale_code = l.locale_code
                LEFT JOIN x_profile_nav_link_locales def_loc ON pn.id = def_loc.nav_id AND def_loc.locale_code = 'en-US'
                WHERE pn.is_active = 1 AND l.is_active = 1;

                CREATE VIEW vw_x_pages_localized AS
                SELECT 
                    p.id AS page_guid,
                    p.page_key,
                    l.locale_code AS requested_locale,
                    COALESCE(req_loc.title, def_loc.title, p.page_key) AS title,
                    COALESCE(req_loc.description, def_loc.description) AS description,
                    p.category
                FROM x_pages p
                CROSS JOIN x_locales l
                LEFT JOIN x_page_locales req_loc ON p.id = req_loc.page_id AND req_loc.locale_code = l.locale_code
                LEFT JOIN x_page_locales def_loc ON p.id = def_loc.page_id AND def_loc.locale_code = 'en-US'
                WHERE l.is_active = 1;

                CREATE VIEW vw_x_mappers_localized AS
                SELECT 
                    m.id AS mapper_guid,
                    m.mapper_key,
                    m.source_type,
                    m.source_name,
                    l.locale_code AS requested_locale,
                    COALESCE(req_loc.display_name, def_loc.display_name, m.mapper_key) AS display_name,
                    COALESCE(req_loc.description, def_loc.description) AS description
                FROM x_mappers m
                CROSS JOIN x_locales l
                LEFT JOIN x_mapper_locales req_loc ON m.id = req_loc.mapper_id AND req_loc.locale_code = l.locale_code
                LEFT JOIN x_mapper_locales def_loc ON m.id = def_loc.mapper_id AND def_loc.locale_code = 'en-US'
                WHERE l.is_active = 1;

                CREATE VIEW vw_x_mapper_flavors_localized AS
                SELECT 
                    mf.id AS flavor_guid,
                    mf.flavor_key,
                    mf.mapper_id,
                    m.mapper_key,
                    l.locale_code AS requested_locale,
                    COALESCE(req_loc.display_name, def_loc.display_name, mf.flavor_key) AS display_name,
                    COALESCE(req_loc.description, def_loc.description) AS description
                FROM x_mapper_flavors mf
                JOIN x_mappers m ON mf.mapper_id = m.id
                CROSS JOIN x_locales l
                LEFT JOIN x_mapper_flavor_locales req_loc ON mf.id = req_loc.flavor_id AND req_loc.locale_code = l.locale_code
                LEFT JOIN x_mapper_flavor_locales def_loc ON mf.id = def_loc.flavor_id AND def_loc.locale_code = 'en-US'
                WHERE l.is_active = 1;

                CREATE VIEW vw_x_mapper_flavor_fields_localized AS
                SELECT 
                    mff.id AS flavor_field_guid,
                    mff.flavor_id,
                    mf.flavor_key,
                    mff.mapper_field_id,
                    'Field' AS mapper_field_name,
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
                CROSS JOIN x_locales l
                LEFT JOIN x_mapper_flavor_field_locales req_loc ON mff.id = req_loc.field_id AND req_loc.locale_code = l.locale_code
                LEFT JOIN x_mapper_flavor_field_locales def_loc ON mff.id = def_loc.field_id AND def_loc.locale_code = 'en-US'
                WHERE l.is_active = 1;

                CREATE VIEW vw_x_forms_localized AS
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
                    COALESCE(req_loc.submit_button_text, def_loc.submit_button_text, 'Submit') AS submit_button_text,
                    f.is_editable,
                    f.label_position,
                    f.grid_cols,
                    f.show_reset_button
                FROM x_forms f
                LEFT JOIN x_mapper_flavors mf ON f.flavor_id = mf.id
                CROSS JOIN x_locales l
                LEFT JOIN vw_x_mapper_flavors_localized vmf ON mf.id = vmf.flavor_guid AND vmf.requested_locale = l.locale_code
                LEFT JOIN x_form_locales req_loc ON f.id = req_loc.form_id AND req_loc.locale_code = l.locale_code
                LEFT JOIN x_form_locales def_loc ON f.id = def_loc.form_id AND def_loc.locale_code = 'en-US'
                WHERE l.is_active = 1;

                CREATE VIEW vw_x_page_forms_localized AS
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
                LEFT JOIN vw_x_pages_localized vp ON p.id = vp.page_guid AND vp.requested_locale = l.locale_code
                LEFT JOIN vw_x_forms_localized vf ON f.id = vf.form_guid AND vf.requested_locale = l.locale_code
                WHERE pf.is_active = 1 AND l.is_active = 1;

                INSERT INTO x_locales (locale_code, display_name, is_default, is_active) VALUES ('en-US', 'English', 1, 1), ('es-ES', 'Spanish', 0, 1);
                
                INSERT INTO x_site_nav_links (id, link_key, icon, route, badge_count, display_order) VALUES ('11111111-1111-1111-1111-111111111101', 'nav-dashboard', 'home', '/dashboard', 4, 1);
                INSERT INTO x_site_nav_link_locales (nav_id, locale_code, label) VALUES ('11111111-1111-1111-1111-111111111101', 'en-US', 'Inbox & Dashboard'), ('11111111-1111-1111-1111-111111111101', 'es-ES', 'Bandeja de Entrada');

                INSERT INTO x_profile_nav_links (id, link_key, icon, url, display_order) VALUES ('22222222-2222-2222-2222-222222222201', 'profile-settings', 'user', '/profile', 1);
                INSERT INTO x_profile_nav_link_locales (nav_id, locale_code, label) VALUES ('22222222-2222-2222-2222-222222222201', 'en-US', 'My Profile');

                INSERT INTO x_pages (id, page_key, category) VALUES ('33333333-3333-3333-3333-333333333301', 'dashboard', 'General');
                INSERT INTO x_page_locales (page_id, locale_code, title, description) VALUES ('33333333-3333-3333-3333-333333333301', 'en-US', 'Operational Dashboard', 'Main Workspace');

                INSERT INTO x_mappers (id, mapper_key, source_type, source_name) VALUES ('66666666-6666-6666-6666-666666666601', 'mapper-asset-master', 'TABLE', 'dbo.Assets');
                INSERT INTO x_mapper_locales (mapper_id, locale_code, display_name, description) VALUES ('66666666-6666-6666-6666-666666666601', 'en-US', 'Asset Master Mapper', 'Full Asset Repository');

                INSERT INTO x_mapper_flavors (id, flavor_key, mapper_id) VALUES ('77777777-7777-7777-7777-777777777701', 'flavor-asset-registration', '66666666-6666-6666-6666-666666666601');
                INSERT INTO x_mapper_flavor_locales (flavor_id, locale_code, display_name, description) VALUES ('77777777-7777-7777-7777-777777777701', 'en-US', 'Asset Registration Flavor', 'Field Subset');

                INSERT INTO x_mapper_flavor_fields (id, flavor_id, key_name, field_type, is_editable, display_order) VALUES ('88888888-8888-8888-8888-888888888801', '77777777-7777-7777-7777-777777777701', 'assetTag', 'text', 1, 1);
                INSERT INTO x_mapper_flavor_field_locales (field_id, locale_code, label, placeholder) VALUES ('88888888-8888-8888-8888-888888888801', 'en-US', 'Asset Tag Number', 'e.g. AST-2026');

                INSERT INTO x_forms (id, form_key, flavor_id, form_type, visible_clause, is_editable) VALUES ('44444444-4444-4444-4444-444444444401', 'asset-create', '77777777-7777-7777-7777-777777777701', 'standard', 'user.isAuthenticated', 1);
                INSERT INTO x_form_locales (form_id, locale_code, caption, title, submit_button_text) VALUES ('44444444-4444-4444-4444-444444444401', 'en-US', 'New Asset Registration Form', 'Asset Registration', 'Save Asset');

                INSERT INTO x_page_forms (id, page_id, form_id, display_order) VALUES ('55555555-5555-5555-5555-555555555501', '33333333-3333-3333-3333-333333333301', '44444444-4444-4444-4444-444444444401', 1);
            ";
            cmd.ExecuteNonQuery();
        }

        [Fact]
        public async Task Dapper_QueriesSiteNavLinks_WithLocalizationAndFallback()
        {
            IEnumerable<XSiteNavLinkEntity> enLinks = await _repository.GetSiteNavLinksAsync("en-US");
            IEnumerable<XSiteNavLinkEntity> esLinks = await _repository.GetSiteNavLinksAsync("es-ES");

            Assert.NotEmpty(enLinks);
            Assert.NotEmpty(esLinks);

            XSiteNavLinkEntity enLink = Assert.Single(enLinks);
            XSiteNavLinkEntity esLink = Assert.Single(esLinks);

            Assert.Equal("Inbox & Dashboard", enLink.Label);
            Assert.Equal("Bandeja de Entrada", esLink.Label);
        }

        [Fact]
        public async Task Dapper_QueriesPages_ReturnsPageMetadata()
        {
            XPageEntity? page = await _repository.GetPageByKeyAsync("dashboard", "en-US");

            Assert.NotNull(page);
            Assert.Equal("dashboard", page.PageKey);
            Assert.Equal("Operational Dashboard", page.Title);
        }

        [Fact]
        public async Task Dapper_QueriesFormsForPage_ReturnsFormAndVisibleClause()
        {
            IEnumerable<XFormEntity> forms = await _repository.GetFormsForPageAsync("dashboard", "en-US");

            Assert.NotEmpty(forms);
            XFormEntity form = Assert.Single(forms);

            Assert.Equal("asset-create", form.FormKey);
            Assert.Equal("New Asset Registration Form", form.Caption);
            Assert.Equal("user.isAuthenticated", form.VisibleClause);
        }

        [Fact]
        public async Task Dapper_QueriesMappersAndFlavors_ReturnsFlavorFields()
        {
            IEnumerable<XMapperFlavorFieldEntity> fields = await _repository.GetFlavorFieldsAsync("flavor-asset-registration", "en-US");

            Assert.NotEmpty(fields);
            XMapperFlavorFieldEntity field = Assert.Single(fields);

            Assert.Equal("assetTag", field.KeyName);
            Assert.Equal("Asset Tag Number", field.Label);
            Assert.Equal("text", field.FieldType);
            Assert.True(field.IsEditable);
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
