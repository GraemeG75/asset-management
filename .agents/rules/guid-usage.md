# GUID Usage Rules

## 1. Always Use Real Valid GUIDs
- Never generate mock or synthetic identifiers that are not valid GUIDs (such as `"USR-2026-4402"`, `"TASK-9901"`, or non-UUID strings) when an entity key or `UNIQUEIDENTIFIER` is required.
- Always use standard, RFC 4122 compliant UUID v4 strings or actual `Guid.NewGuid()` / `Guid.Parse(...)` instances (e.g. `"e8a719c2-570a-4a2e-9d2a-8d7d91e84321"`).
- All primary keys and foreign keys for database entities, metadata records, and test fixtures must use real, valid GUID format (`UNIQUEIDENTIFIER`).
