# Inventory Handler Alignment Summary

## Newly Added Handlers

### Seller
- `UnsetSellerAsSystemOwnerCommandHandler`
- `GetSellerSummaryQueryHandler`

### Warehouse
- `GetWarehouseSummaryQueryHandler`
- `GetWarehouseWithLocationsQueryHandler`

### Location
- `MoveLocationToWarehouseCommandHandler`
- `GetLocationsByTypeQueryHandler`

### StockDetail (internal posting/application)
- `ApplyReceiptToStockDetailCommandHandler`
- `ApplyIssueToStockDetailCommandHandler`
- `ApplyTransferToStockDetailCommandHandler`
- `ApplyAdjustmentToStockDetailCommandHandler`
- `ApplyQualityChangeToStockDetailCommandHandler`
- `ApplyReturnToStockDetailCommandHandler`

### StockDetail (query coverage)
- `GetStockDetailsByQualityStatusQueryHandler`
- `GetStockDetailsByLotBatchQueryHandler`
- `GetAvailableStockBucketsQueryHandler`
- `GetEmptyStockBucketsQueryHandler`
- `GetStockAgingQueryHandler`
- `GetLowStockBucketsQueryHandler`

## Refactored Handlers
- `AdjustStockDetailCommandHandler` was changed to reject direct quantity mutation and enforce official posting flow.

## Renamed Handlers

### ProductVariant command handlers
- `ActivateVariantCommandHandler` -> `ActivateProductVariantCommandHandler`
- `DeactivateVariantCommandHandler` -> `DeactivateProductVariantCommandHandler`
- `DeleteVariantCommandHandler` -> `DeleteProductVariantCommandHandler`
- `ChangeVariantTrackingPolicyCommandHandler` -> `ChangeProductVariantTrackingPolicyCommandHandler`
- `ChangeVariantBaseUomCommandHandler` -> `ChangeProductVariantBaseUomCommandHandler`
- `LockVariantInventoryMovementCommandHandler` -> `LockProductVariantInventoryMovementCommandHandler`

### ProductVariant query handlers
- `GetVariantByIdQueryHandler` -> `GetProductVariantByIdQueryHandler`
- `GetVariantsByProductIdQueryHandler` -> `GetProductVariantsByProductIdQueryHandler`
- `SearchVariantsQueryHandler` -> `SearchProductVariantsQueryHandler`
- `GetVariantBySkuQueryHandler` -> `GetProductVariantBySkuQueryHandler`
- `GetVariantByBarcodeQueryHandler` -> `GetProductVariantByBarcodeQueryHandler`
- `GetVariantFullDetailsQueryHandler` -> `GetProductVariantFullDetailsQueryHandler`

### SerialItem command handlers
- `AssignSerialToStockDetailCommandHandler` -> `AssignSerialItemToStockDetailCommandHandler`

## Domain Decisions and Assumptions
- `StockDetail` remains source of truth for physical stock, but direct public adjustment semantics were disabled (`AdjustStockDetailCommandHandler` now rejects).
- Official stock mutation path is preserved as posting-driven (`InventoryDocument` posting flow creating/marking `InventoryTransaction` and applying stock effects).
- `Location` move is supported because domain entity exposes `ChangeWarehouse`; therefore `MoveLocationToWarehouseCommandHandler` was added.
- Seller system owner is reversible because domain model stores it as mutable boolean; therefore `UnsetSellerAsSystemOwnerCommandHandler` was added.
- `Reservation` and `Fulfillment` were not changed to mutate physical stock directly.
- Low-stock query uses threshold-based operational semantics (`QuantityOnHand > 0 && QuantityOnHand <= ThresholdQuantity`).
- Stock aging is computed from `FirstReceivedAt` against `AsOfUtc` (default: `DateTime.UtcNow`).
