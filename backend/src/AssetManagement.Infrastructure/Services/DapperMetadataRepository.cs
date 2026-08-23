using System.Data;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using Dapper;

namespace AssetManagement.Infrastructure.Services
{
    public class DapperMetadataRepository : IMetadataRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        static DapperMetadataRepository()
        {
            SqlMapper.AddTypeHandler(new GuidTypeHandler());
        }

        public DapperMetadataRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<XSiteNavLinkEntity>> GetSiteNavLinksAsync(string locale = "en-US")
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    nav_id AS NavId, 
                    link_key AS LinkKey, 
                    requested_locale AS RequestedLocale, 
                    label AS Label, 
                    icon AS Icon, 
                    route AS Route, 
                    badge_count AS BadgeCount, 
                    category AS Category, 
                    display_order AS DisplayOrder 
                FROM vw_x_site_nav_links_localized 
                WHERE requested_locale = @locale 
                ORDER BY display_order ASC;";
            
            IEnumerable<XSiteNavLinkEntity> results = await connection.QueryAsync<XSiteNavLinkEntity>(sql, new { locale });
            return results;
        }

        public async Task<IEnumerable<XProfileNavLinkEntity>> GetProfileNavLinksAsync(string locale = "en-US")
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    nav_id AS NavId, 
                    link_key AS LinkKey, 
                    requested_locale AS RequestedLocale, 
                    label AS Label, 
                    icon AS Icon, 
                    url AS Url, 
                    badge AS Badge, 
                    badge_color AS BadgeColor, 
                    display_order AS DisplayOrder 
                FROM vw_x_profile_nav_links_localized 
                WHERE requested_locale = @locale 
                ORDER BY display_order ASC;";
            
            IEnumerable<XProfileNavLinkEntity> results = await connection.QueryAsync<XProfileNavLinkEntity>(sql, new { locale });
            return results;
        }

        public async Task<IEnumerable<XPageEntity>> GetPagesAsync(string locale = "en-US")
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    page_guid AS PageGuid, 
                    page_key AS PageKey, 
                    requested_locale AS RequestedLocale, 
                    title AS Title, 
                    description AS Description, 
                    category AS Category 
                FROM vw_x_pages_localized 
                WHERE requested_locale = @locale;";
            
            IEnumerable<XPageEntity> results = await connection.QueryAsync<XPageEntity>(sql, new { locale });
            return results;
        }

        public async Task<XPageEntity?> GetPageByKeyAsync(string pageKey, string locale = "en-US")
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    page_guid AS PageGuid, 
                    page_key AS PageKey, 
                    requested_locale AS RequestedLocale, 
                    title AS Title, 
                    description AS Description, 
                    category AS Category 
                FROM vw_x_pages_localized 
                WHERE page_key = @pageKey AND requested_locale = @locale;";
            
            XPageEntity? page = await connection.QueryFirstOrDefaultAsync<XPageEntity>(sql, new { pageKey, locale });
            return page;
        }

        public async Task<IEnumerable<XFormEntity>> GetFormsForPageAsync(string pageKey, string locale = "en-US")
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    pf.form_id AS FormGuid, 
                    pf.form_key AS FormKey, 
                    vf.flavor_id AS FlavorId, 
                    vf.flavor_key AS FlavorKey, 
                    vf.flavor_display_name AS FlavorDisplayName, 
                    pf.form_type AS FormType, 
                    pf.effective_visible_clause AS VisibleClause, 
                    pf.requested_locale AS RequestedLocale, 
                    pf.form_caption AS Caption, 
                    pf.form_title AS Title, 
                    vf.description AS Description, 
                    vf.form_info AS FormInfo, 
                    vf.submit_button_text AS SubmitButtonText, 
                    pf.is_editable AS IsEditable, 
                    pf.label_position AS LabelPosition, 
                    pf.grid_cols AS GridCols 
                FROM vw_x_page_forms_localized pf 
                LEFT JOIN vw_x_forms_localized vf ON pf.form_id = vf.form_guid AND pf.requested_locale = vf.requested_locale 
                WHERE pf.page_key = @pageKey AND pf.requested_locale = @locale 
                ORDER BY pf.display_order ASC;";
            
            IEnumerable<XFormEntity> results = await connection.QueryAsync<XFormEntity>(sql, new { pageKey, locale });
            return results;
        }

        public async Task<XFormEntity?> GetFormByKeyAsync(string formKey, string locale = "en-US")
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    form_guid AS FormGuid, 
                    form_key AS FormKey, 
                    flavor_id AS FlavorId, 
                    flavor_key AS FlavorKey, 
                    flavor_display_name AS FlavorDisplayName, 
                    form_type AS FormType, 
                    visible_clause AS VisibleClause, 
                    requested_locale AS RequestedLocale, 
                    caption AS Caption, 
                    title AS Title, 
                    description AS Description, 
                    form_info AS FormInfo, 
                    submit_button_text AS SubmitButtonText, 
                    is_editable AS IsEditable, 
                    label_position AS LabelPosition, 
                    grid_cols AS GridCols, 
                    show_reset_button AS ShowResetButton 
                FROM vw_x_forms_localized 
                WHERE form_key = @formKey AND requested_locale = @locale;";
            
            XFormEntity? form = await connection.QueryFirstOrDefaultAsync<XFormEntity>(sql, new { formKey, locale });
            return form;
        }

        public async Task<IEnumerable<XMapperEntity>> GetMappersAsync(string locale = "en-US")
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    mapper_guid AS MapperGuid, 
                    mapper_key AS MapperKey, 
                    source_type AS SourceType, 
                    source_name AS SourceName, 
                    requested_locale AS RequestedLocale, 
                    display_name AS DisplayName, 
                    description AS Description 
                FROM vw_x_mappers_localized 
                WHERE requested_locale = @locale;";
            
            IEnumerable<XMapperEntity> results = await connection.QueryAsync<XMapperEntity>(sql, new { locale });
            return results;
        }

        public async Task<IEnumerable<XMapperFlavorEntity>> GetMapperFlavorsAsync(string mapperKey, string locale = "en-US")
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    flavor_guid AS FlavorGuid, 
                    flavor_key AS FlavorKey, 
                    mapper_id AS MapperId, 
                    mapper_key AS MapperKey, 
                    requested_locale AS RequestedLocale, 
                    display_name AS DisplayName, 
                    description AS Description 
                FROM vw_x_mapper_flavors_localized 
                WHERE mapper_key = @mapperKey AND requested_locale = @locale;";
            
            IEnumerable<XMapperFlavorEntity> results = await connection.QueryAsync<XMapperFlavorEntity>(sql, new { mapperKey, locale });
            return results;
        }

        public async Task<IEnumerable<XMapperFlavorFieldEntity>> GetFlavorFieldsAsync(string flavorKey, string locale = "en-US")
        {
            using IDbConnection connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    flavor_field_guid AS FlavorFieldGuid, 
                    flavor_id AS FlavorId, 
                    flavor_key AS FlavorKey, 
                    mapper_field_id AS MapperFieldId, 
                    mapper_field_name AS MapperFieldName, 
                    key_name AS KeyName, 
                    field_type AS FieldType, 
                    is_editable AS IsEditable, 
                    is_readonly AS IsReadonly, 
                    is_disabled AS IsDisabled, 
                    requested_locale AS RequestedLocale, 
                    label AS Label, 
                    placeholder AS Placeholder, 
                    default_value AS DefaultValue, 
                    help_text AS HelpText, 
                    display_order AS DisplayOrder, 
                    grid_cols AS GridCols, 
                    custom_css_class AS CustomCssClass 
                FROM vw_x_mapper_flavor_fields_localized 
                WHERE flavor_key = @flavorKey AND requested_locale = @locale 
                ORDER BY display_order ASC;";
            
            IEnumerable<XMapperFlavorFieldEntity> results = await connection.QueryAsync<XMapperFlavorFieldEntity>(sql, new { flavorKey, locale });
            return results;
        }
    }
}
