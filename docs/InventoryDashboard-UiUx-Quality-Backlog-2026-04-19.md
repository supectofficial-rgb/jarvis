# Inventory Dashboard UI/UX Quality Backlog (60-point)

## Purpose
- Keep UI/UX quality improvements continuous across the project.
- Treat this as the baseline backlog and review gate for each catalog release.
- Do focused, incremental hardening without rewriting working flows.

## Workflow (Always On)
1. Pick 3-7 items per sprint from highest priority open items.
2. Keep changes scoped to Categories / Products / Variants unless explicitly expanded.
3. Preserve existing business behavior and API contracts.
4. Add/update acceptance checks in PR notes for each item touched.
5. Re-run this checklist after every release and move completed items to `Done`.

## Status Legend
- `TODO`: not started
- `IN_PROGRESS`: partially implemented
- `DONE`: implemented and verified in UI
- `DEFERRED`: intentionally postponed with reason

## Priority 1 (Critical)
1. `DONE` Page goal model per screen (one primary goal + secondary actions).
2. `DONE` Task-oriented layout (workspace feel, not disconnected forms).
3. `DONE` Two-pane stable layout for Categories/Products/Variants.
4. `DONE` Strong visual hierarchy on all main pages.
5. `DONE` Strict button semantics: primary/secondary/destructive/neutral.
6. `DONE` Better section containers and card composition.
7. `DONE` Category tree polish (depth, expand/collapse quality, selected state).
8. `DONE` Parent category displayed as full path.
9. `DONE` Row-action-driven category operations.
10. `DONE` Rule management moved to grid/modal/drawer UX.
11. `DONE` Enterprise-grade product table (sortable, action menu, columns).
12. `DONE` Product form grouped by base info + attributes.
13. `DONE` Clear product attribute labels (source/type/required/inherited).
14. `DONE` Product completion status indicator (progress + actionable gaps).
15. `DONE` High-risk modal warning for change category action.
16. `DONE` Variant page polish with list + detail tabs structure.
17. `DONE` Tracking policy help text (domain guidance).
18. `DONE` Strong lock-state UX for variants (readonly + reasons).
19. `DONE` Better UOM conversion UX (table + modal + validation).
20. `DONE` Rich variant attribute table row actions and metadata.

## Priority 2 (High)
21. `DONE` Replace heavy plain selects with searchable selects.
22. `DONE` Improve dynamic field rendering by type.
23. `DONE` Meaningful placeholders and helper texts.
24. `IN_PROGRESS` Consistent Persian localization strategy (single-language or controlled bilingual).
25. `DONE` Add professional Persian UI font stack.
26. `IN_PROGRESS` Direction-safe RTL spacing utilities (remove ad-hoc mirroring hacks).
27. `DONE` Better numeric/code rendering for RTL readability.
28. `DONE` Permission-denied reason hints on disabled actions.
29. `DONE` Dedicated access-denied state panel (not only temp error message).
30. `DONE` Enterprise table UX baseline (sticky header, skeleton, summary, empty states).
31. `DONE` Smart/collapsible filters with chips and reset behavior.
32. `DONE` Bulk action readiness design for admin workflows.
33. `DONE` Toast-based success/error + contextual validation messaging.
34. `DONE` Domain-aware destructive confirmation messages.
35. `DONE` Accurate unsaved-changes guard (true dirty-only behavior).

## Priority 3 (Important Polish)
36. `IN_PROGRESS` Unified iconography for key actions.
37. `DONE` Standard badge system (active/inactive/inherited/locked/...).
38. `DONE` Strong empty states with clear next action.
39. `DONE` Strong page context header/breadcrumb chips.
40. `DONE` Desktop-first with tablet-safe responsive behavior.
41. `DONE` Extract reusable partials/components for repeated UI blocks.
42. `IN_PROGRESS` Move heavy UI state logic out of views where needed.
43. `DONE` Introduce design tokens (color, spacing, radius, elevation, typography).

## Priority 4 (Advanced Enhancements)
44. `DONE` Searchable select everywhere relevant.
45. `DONE` Persian font and typography polish completed end-to-end.
46. `DONE` Fully polished category tree behavior.
47. `DONE` Contextual detail pane on complex pages.
48. `DONE` Modal-based row actions for advanced operations.
49. `DONE` Better empty states everywhere.
50. `DONE` Domain-aware confirmations everywhere.
51. `DONE` Better permission hints everywhere.
52. `IN_PROGRESS` Full language consistency across UI.
53. `DONE` Fully unified badge system.
54. `DONE` Table UX enhancements fully applied.
55. `DONE` Advanced filter UX complete.
56. `DONE` Sticky action bars in long forms.
57. `DONE` Keyboard-friendly admin interactions.
58. `DONE` Visual completion indicators across forms.
59. `DONE` Lightweight onboarding/help for complex flows.
60. `DONE` Audit/context chips in detail panels.

## Immediate Baseline Safeguards (Applied)
- Controller action surfaces restored for:
  - `CategoryManagementController`
  - `ProductManagementController`
  - `VariantManagementController`
- Legacy route controller kept and documented as compatibility layer:
  - `CatalogManagementLegacyController`

## Definition of Done Per Item
- UI behavior implemented and visible in the relevant page.
- No regression in current create/update flows.
- Permission and validation feedback remain intact.
- Route/action still reachable and anti-forgery preserved for POST.
- State persistence or user feedback behavior tested for failure/redirect paths where applicable.
