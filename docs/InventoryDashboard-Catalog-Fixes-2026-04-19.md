# InventoryDashboard Catalog Fixes (2026-04-19)

## Newly Added
- `RemoveProductAttributeValue` action in `CatalogManagementController`.
- `RemoveVariantAttributeValue` action in `CatalogManagementController`.
- API service methods:
  - `RemoveProductAttributeValueAsync`
  - `RemoveVariantAttributeValueAsync`
  - `GetUnitOfMeasureLookupAsync`
- Product UI:
  - `BaseSku` field in create/update form.
  - `Default UOM` dropdown from lookup.
  - Remove button per product attribute row.
- Variant UI:
  - `Base UOM` dropdown from lookup (no free-text).
  - Remove button per variant attribute row.

## Refactored
- `ApiService` inventory routes normalized to `api/InventoryService/...` controllers.
- `ApiService` request payloads aligned with backend command contracts:
  - Category: `Code`, `DisplayOrder`, `ParentCategoryRef`
  - Product: `CategoryRef`, `BaseSku`, `DefaultUomRef`, `TaxCategoryRef`
  - ProductVariant: `VariantSku`, `BaseUomRef`, `TrackingPolicy`
  - Product/Variant attribute set: route includes `{attributeRef}`, payload includes `OptionRef`
- Category attribute creation flow changed to:
  1. Create attribute definition
  2. Add category attribute rule
- Category/Product/Variant models in dashboard updated for real backend DTO mapping.

## Renamed / Structural Normalization
- Variant form field changed from `BaseUom` (free text) to `BaseUomRef` (lookup id).
- Category form now includes `Code` and `DisplayOrder`.
- Attribute form now includes `Code`, `Scope`, `IsVariant`, and `DisplayOrder`.
- Attribute option form now includes `DisplayOrder`.

## Domain / Functional Assumptions
- Category/Product/Variant create/update must supply required identity fields (`Code`, `BaseSku`, `UOM refs`).
- Attribute options are treated as `Option` data type in backend (`Text/Number/Boolean/Date/Option`).
- Attribute remove operations are soft-functional from UI perspective (backend command semantics preserved).

## Not Completed Yet (Next Slice)
- Full lifecycle buttons in UI (activate/deactivate/delete/move/change-category/change-tracking/lock).
- Full CategoryAttributeRule editor (required/variant/display order override toggles as editable grid).
- Product/Variant editor using backend `editor-data` endpoints (for richer completion-state UX).
- Permission-aware action-level enforcement in panel (beyond menu visibility).
