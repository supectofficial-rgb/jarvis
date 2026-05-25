# Inventory Document UI Standard

This document records the standard behavior for all Inventory document screens
in `Insurance.InventoryDashboard.Panel`.

## Scope

Applies to:

- Receipt
- Issue
- Transfer
- Adjustment
- Return
- Conversion
- Quality Change

## Core Rules

1. The `Create` tab is header-first.
   - No inline line-entry section in the main create tab.
   - The document must be creatable without exposing line details in the header UI.

2. The `List` tab is the operational surface.
   - Each document list should expose an `Action` button.
   - `Action` opens a modal for status changes.
   - The status modal must work through AJAX and submit without full page reload.

3. Details must be loaded independently.
   - Do not reuse hidden markup from the main page for details.
   - Each details modal must be loaded by its own server request.
   - The response should be rendered into the modal body.

4. Line add/edit/delete must refresh the modal list.
   - Save line actions must return the updated modal body.
   - Delete line actions must return the updated modal body.
   - Edit line actions must reload the same modal body with the target line in edit mode.

5. Search inputs must follow the searchable-select standard.
   - Use AJAX-backed searchable selects for reference selection.
   - The user types part of a code/name and receives matching results below the field.
   - Avoid custom one-off lookup inputs when the standard searchable select works.

6. Serial-aware lines must behave consistently.
   - If a location/variant has serials, show the serial list.
   - When serials are selected, quantity must sync automatically.
   - Quantity must be readonly in serial-tracked mode.

7. Localization must be used for labels, buttons, and empty states.
   - No hard-coded UI text in views when a localization key exists.
   - Add keys to both Persian and English dictionaries.

8. Status and action text must be consistent.
   - Use the same action naming across document pages.
   - Prefer `Action` for the status/status-change modal entry point.

## Search Input Standard

Search fields across document screens must use the same lookup pattern.

### Standard Pattern

1. Use an AJAX-backed searchable select or equivalent lookup widget.
2. The user types a partial code or name.
3. The UI requests matching records from the server.
4. Matching options are rendered below the field.
5. The user selects one option from the returned list.
6. Dependent data refreshes after selection.

### Independent Search Filters on List Tab

Some search filters on the `List` tab are not derived from the current document header.
They are independent document discovery filters and should behave as such.

Examples:

- Variant filter
  - The user selects a variant using the standard searchable-select pattern.
  - The system returns documents whose line items include that variant in their details.
  - This filter is not tied to the active document header; it is a discovery filter over documents.

- Warehouse / Location filter
  - The user selects a warehouse or a location using the same searchable-select pattern.
  - The system returns documents whose line items contain variants stored in that warehouse or location.
  - The filter works across document details, not only the header fields.

Implementation note:

- Keep the UX consistent with the existing searchable-select standard.
- The meaning of the filter is broader than the header.
- The query backend should search the document detail rows and not only the header row when these filters are used.

### Variant Search Standard

- The variant lookup in document forms must use the searchable-select pattern.
- The user can search by code or name.
- The lookup must not rely on a plain text input plus ad hoc button/search panel.
- The selected variant drives the next dependent lookups, such as locations or serials.

### Return Document Reference Search Standard

- In the Return document create flow, the reference document field must use the same searchable-select pattern.
- The user types part of the `Issue` document number.
- The UI requests matching `Issue` documents from the server.
- Matching documents are shown in a dropdown list below the field.
- After selection, the reference document is loaded and dependent fields refresh.
- Hidden seller/warehouse values may be auto-derived from the selected reference document when the workflow requires it.

## Return Document Standard

Return document pages are the reference implementation for the current standard:

- Reference document selection uses a searchable AJAX select.
- The details modal is loaded from a dedicated endpoint.
- The line list inside the modal refreshes after add/edit/delete.
- Warehouse and seller filters are hidden in the create flow when they are derived from the reference issue document.

## Implementation Notes

- Modal content should be fetched with a dedicated request.
- Modal refresh should replace only the modal body.
- Keep the main page intact unless the user explicitly leaves the modal flow.
- If a page needs a specialized action flow, it should still follow the same modal refresh pattern.

## Acceptance Checklist

- Create tab is header-only.
- List tab has `Action`.
- Details are loaded via AJAX endpoint.
- Line save/delete refresh the modal body.
- Search inputs are AJAX-backed searchable selects.
- Serial-driven lines auto-sync quantity.
- All visible labels use localization keys.
