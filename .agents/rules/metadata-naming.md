# Metadata Naming Convention (`x_` Prefix)

## 1. Naming Rule for Metadata Entities & Tables
- ALL database tables, views, schema structures, and data structures pertaining to metadata MUST start with the prefix `x_` (e.g. `x_locales`, `x_site_nav_links`, `x_pages`, `x_page_forms`, `x_forms`, `x_mappers`, `x_mapper_flavors`, `x_form_fields`, `x_form_field_validators`, `x_form_field_options`).
- ALL database views serving metadata MUST start with `vw_x_` (e.g. `vw_x_pages_localized`, `vw_x_page_forms_localized`, `vw_x_forms_localized`, `vw_x_mappers_localized`, `vw_x_mapper_flavors_localized`).

## 2. Rationale & Separation
- Using the `x_` prefix isolates all metadata entities from domain transaction entities, enabling clear separation, migration, or extraction of metadata structures whenever needed.

## 3. Many-to-Many Page & Form Relationship (`x_page_forms`)
- A single form can exist on multiple pages, and a page can contain multiple forms.
- Page to form mappings are stored in the dedicated junction metadata table `x_page_forms` (`id` UNIQUEIDENTIFIER, `page_id` UNIQUEIDENTIFIER, `form_id` UNIQUEIDENTIFIER, `visible_clause` NVARCHAR(MAX), `display_order` INT, `is_active` BIT).

## 4. Form Visible Clauses (`visible_clause`)
- Forms and page-form mappings include a `visible_clause` column (NVARCHAR(MAX)) containing conditional evaluation expressions determining if the form should be rendered.

## 5. Mappers & Mapper Flavors Architecture (`x_mappers` & `x_mapper_flavors`)
- **Mapper (`x_mappers`)**: Defines an underlying database data source (Table, View, or Stored Procedure) containing the master set of all available fields.
- **Mapper Flavor (`x_mapper_flavors`)**: A reduced, customized subset of fields from a Mapper. Defines field-level behavior (editable vs readonly, input component type, grid span, validation).
- **Flavor Uniqueness**: `flavor_key` is NOT globally unique across the database; it is scoped per mapper (`CONSTRAINT UQ_x_mapper_flavors_mapper_flavor UNIQUE (mapper_id, flavor_key)`).
- **Localization**: Mappers, Flavors, and Flavor Fields MUST have corresponding `_locales` tables (`x_mapper_locales`, `x_mapper_flavor_locales`, `x_mapper_flavor_field_locales`) with views providing automatic `en-US` default fallbacks.
