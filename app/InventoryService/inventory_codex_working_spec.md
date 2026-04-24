# Inventory Microservice – Codex Working Specification

## Purpose

This document is the authoritative working specification for implementing the **Inventory Microservice**.  
Use it as the project context and execution guide.

The goal of this service is to manage:

- physical inventory truth
- stock movement journal
- inventory documents
- reservation
- fulfillment execution handoff
- return inventory processing
- source tracing for future financial valuation

This service **does not implement accounting**.  
However, it must preserve enough source traceability so a future **Financial Microservice** can calculate valuation, COGS, and profit/loss independently.

---

# 1. Architectural Principles

## 1.1 Core Truth Model

The service must follow this rule:

- `StockDetail` = source of truth for current physical inventory
- `InventoryTransaction` = immutable journal / audit of inventory movements
- `Projection` models = read-optimized views only, never source of truth

## 1.2 Official Inventory Posting Flow

All official inventory movements must ultimately go through this chain:

`InventoryDocument -> InventoryTransaction -> StockDetail`

For order-driven outbound:

`Order -> Reservation -> Fulfillment -> InventoryDocument -> InventoryTransaction -> StockDetail`

## 1.3 Reservation Rule

Reservation does **not** reduce physical stock.  
Reservation only affects **availability**.

## 1.4 Fulfillment Rule

Fulfillment is an **execution layer**.  
It must **not directly mutate stock**.

Stock changes only happen through posted inventory transactions.

## 1.5 Financial Boundary

This service must **not** own:

- accounting journal
- ledger entries
- debit/credit
- P&L
- COGS calculation logic

This service **must** own:

- quantity truth
- movement truth
- source tracing of inbound-to-outbound consumption

This is required so the future Financial service can determine valuation based on receipt-linked consumption.

---

# 2. Scope of This Microservice

Included:

- Catalog references needed by inventory
- Warehouse and locations
- Stock truth
- Inventory journal
- Inventory documents
- Reservation
- Fulfillment core
- Return core
- Source balance / source allocation / source consumption
- Inventory projections

Excluded for now:

- Procurement
- Supplier management
- Purchase invoice workflow
- Accounting
- Shipping carrier domain
- Forecasting
- Advanced WMS features such as wave picking
- Replenishment planning

---

# 3. Aggregate Roots and Entities

## 3.1 Catalog Domain

### Category (Aggregate Root)
Fields:
- CategoryId
- ParentCategoryRef
- Code
- Name
- IsActive
- DisplayOrder

### AttributeDefinition (Aggregate Root)
Fields:
- AttributeId
- Code
- Name
- DataType
- Scope(Product|Variant|Both)
- IsActive

### AttributeOption (Entity)
Fields:
- OptionId
- AttributeRef
- Value
- DisplayOrder
- IsActive

### CategoryAttributeRule (Entity)
Fields:
- CategoryAttributeRuleId
- CategoryRef
- AttributeRef
- IsRequired
- IsVariant
- DisplayOrder
- IsOverridden
- IsActive

### Product (Aggregate Root)
Fields:
- ProductId
- CategoryRef
- BaseSku
- Name
- DefaultUomRef
- TaxCategoryRef
- IsActive

### ProductAttributeValue (Entity)
Fields:
- ProductAttributeValueId
- ProductRef
- AttributeRef
- Value
- OptionRef

### ProductVariant (Aggregate Root)
Fields:
- VariantId
- ProductRef
- VariantSku
- Barcode
- TrackingPolicy(None|Batch|Serial)
- BaseUomRef
- IsActive

Important:
After first inventory transaction, these fields must be immutable or tightly controlled:
- TrackingPolicy
- BaseUomRef

### VariantAttributeValue (Entity)
Fields:
- VariantAttributeValueId
- VariantRef
- AttributeRef
- Value
- OptionRef

### UnitOfMeasure (Lookup / Aggregate Root)
Fields:
- UomId
- Code
- Name
- Precision

### VariantUomConversion (Entity)
Fields:
- ConversionId
- VariantRef
- FromUomRef
- ToUomRef
- Factor
- RoundingMode
- IsBasePath

---

## 3.2 Seller Domain

### Seller (Aggregate Root)
Fields:
- SellerId
- Code
- Name
- IsSystemOwner
- IsActive

---

## 3.3 Pricing Domain

This is sales pricing only, not inventory valuation.

### PriceType (Lookup)
Fields:
- PriceTypeId
- Code
- Name

### PriceChannel (Lookup)
Fields:
- PriceChannelId
- Code
- Name

### SellerVariantPrice (Aggregate Root)
Fields:
- PriceId
- SellerRef
- VariantRef
- PriceTypeRef
- PriceChannelRef
- Amount
- Currency
- MinQty
- Priority
- EffectiveFrom
- EffectiveTo
- IsActive

### Offer (Entity)
Fields:
- OfferId
- PriceRef
- Name
- DiscountAmount
- DiscountPercent
- MaxQuantity
- Priority
- StartAt
- EndAt
- IsActive

---

## 3.4 Warehouse Domain

### Warehouse (Aggregate Root)
Fields:
- WarehouseId
- Code
- Name
- IsActive

### Location (Aggregate Root)
Fields:
- LocationId
- WarehouseRef
- Aisle
- Rack
- Shelf
- Bin
- LocationCode
- LocationType(Pick|Bulk|Return|Damage|Quarantine)
- IsActive

Invariant:
A location must belong to exactly one warehouse.

### QualityStatus (Lookup)
Fields:
- QualityStatusId
- Code
- Name

Examples:
- Available
- Quarantine
- Damaged
- Expired
- ReturnedInspection

---

## 3.5 Inventory Domain

### StockDetail (Aggregate Root)
Source of truth for physical stock.

Fields:
- StockDetailId
- VariantRef
- SellerRef
- WarehouseRef
- LocationRef
- QualityStatusRef
- LotBatchNo
- QuantityOnHand
- FirstReceivedAt
- LastReceivedAt
- LastUpdatedAt

Bucket key:
`VariantRef + SellerRef + WarehouseRef + LocationRef + QualityStatusRef + LotBatchNo`

Notes:
- `ReservedQuantity` must not be stored here
- `LotBatchNo` must be null for non-batch items unless a specific rule says otherwise

---

### SerialItem (Aggregate Root)
Fields:
- SerialId
- SerialNo
- VariantRef
- SellerRef
- WarehouseRef
- LocationRef
- StockDetailRef
- QualityStatusRef
- LotBatchNo
- Status(Available|Reserved|Issued|Returned|Scrapped|Quarantine)
- DateScannedIn
- LastTransactionRef
- LastUpdatedAt

---

### InventoryTransaction (Aggregate Root)
Immutable inventory journal.

Fields:
- TransactionId
- TransactionNo
- TransactionType(Receipt|Issue|Transfer|Adjustment|Return|QualityChange|ReserveConsume)
- ReferenceType
- ReferenceBusinessId
- WarehouseRef
- SellerRef
- OccurredAt
- PostedAt
- Status(Draft|Posted|Reversed|Cancelled)
- CorrelationId
- IdempotencyKey
- ReasonCode
- ReversedTransactionRef

Rule:
Only `Posted` transactions affect stock.

---

### InventoryTransactionLine (Entity)
Fields:
- LineId
- InventoryTransactionRef
- StockDetailRef
- VariantRef
- InputQty
- InputUomRef
- BaseQtyDelta
- BaseUomRef
- SourceLocationRef
- DestinationLocationRef
- OldQualityStatusRef
- NewQualityStatusRef
- LotBatchNo
- SerialRef
- ReasonCode

Meaning:
- `InputQty` and `InputUomRef` are business input values
- `BaseQtyDelta` is the normalized quantity that actually affects stock

---

## 3.6 Inventory Document Domain

### InventoryDocument (Aggregate Root)
Official entry point for inventory workflows.

Fields:
- DocumentId
- DocumentNo
- DocumentType(Receipt|Issue|Transfer|Adjustment|Return|QualityChange)
- ReferenceType
- ReferenceBusinessId
- WarehouseRef
- SellerRef
- Status(Draft|Approved|Posted|Cancelled|Rejected)
- OccurredAt
- ApprovedAt
- ApprovedBy
- PostedAt
- PostedBy
- PostedTransactionRef
- CorrelationId
- IdempotencyKey
- ReasonCode

---

### InventoryDocumentLine (Entity)
Fields:
- LineId
- DocumentRef
- VariantRef
- Qty
- UomRef
- BaseQty
- BaseUomRef
- SourceLocationRef
- DestinationLocationRef
- QualityStatusRef
- FromQualityStatusRef
- ToQualityStatusRef
- LotBatchNo
- ReasonCode
- AdjustmentDirection(Increase|Decrease)

---

### InventoryDocumentLineSerial (Entity)
Needed for serial-tracked line detail.

Fields:
- LineSerialId
- DocumentLineRef
- SerialRef
- SerialNo

---

## 3.7 Reservation Domain

### InventoryReservation (Aggregate Root)
Fields:
- ReservationId
- OrderRef
- OrderItemRef
- VariantRef
- SellerRef
- WarehouseRef
- ChannelRef
- RequestedQuantity
- AllocatedQuantity
- ConsumedQuantity
- Status(Pending|Confirmed|Consumed|Released|Expired|Rejected)
- ExpiresAt
- ConfirmedAt
- ConsumedAt
- ReleasedAt
- RejectedAt
- CorrelationId
- IdempotencyKey
- ReasonCode

---

### ReservationAllocation (Entity)
Fields:
- AllocationId
- ReservationRef
- StockDetailRef
- VariantRef
- WarehouseRef
- LocationRef
- QualityStatusRef
- LotBatchNo
- SerialRef
- AllocatedQty
- AllocatedAt
- ReleasedQty
- ConsumedQty

Meaning:
Bucket-level or serial-level allocation.

---

### ReservationTransition (Entity)
Fields:
- TransitionId
- ReservationRef
- FromStatus
- ToStatus
- ReasonCode
- ChangedAt
- ChangedBy
- CorrelationId

---

## 3.8 Fulfillment Domain

### Fulfillment (Aggregate Root)
Fields:
- FulfillmentId
- OrderRef
- SellerRef
- WarehouseRef
- ChannelRef
- Status(Pending|Picked|Packed|Shipped|Returned|PartiallyReturned)
- PickedAt
- PackedAt
- ShippedAt
- ReturnedAt
- CreatedAt
- UpdatedAt

### FulfillmentLine (Entity)
Fields:
- LineId
- FulfillmentRef
- VariantRef
- Qty
- UomRef
- BaseQty
- BaseUomRef
- SourceLocationRef
- LotBatchNo
- ReservationAllocationRef
- Status(Pending|Picked|Packed|Shipped|Returned)
- PickedAt
- PackedAt
- ShippedAt
- ReturnedAt

### FulfillmentLineSerial (Entity)
Fields:
- LineSerialId
- FulfillmentLineRef
- SerialRef
- SerialNo

### FulfillmentTransition (Entity)
Fields:
- TransitionId
- FulfillmentRef
- FromStatus
- ToStatus
- ReasonCode
- ChangedAt
- ChangedBy
- CorrelationId

Rule:
Fulfillment is an execution layer, not a stock owner.

---

## 3.9 Return Domain

### ReturnRequest (Aggregate Root)
Fields:
- ReturnId
- OrderRef
- OrderItemRef
- SellerRef
- WarehouseRef
- Status(Requested|Approved|Rejected|Received|Closed)
- ReasonCode
- RequestedAt
- ApprovedAt
- RejectedAt
- ReceivedAt
- ClosedAt
- ApprovedBy
- RejectedBy
- ReceivedBy

### ReturnLine (Entity)
Fields:
- LineId
- ReturnRef
- VariantRef
- Qty
- UomRef
- BaseQty
- BaseUomRef
- LotBatchNo
- ExpectedCondition
- ReceivedCondition
- Disposition(Restock|Quarantine|Scrap)

### ReturnLineSerial (Entity)
Fields:
- LineSerialId
- ReturnLineRef
- SerialRef
- SerialNo

---

## 3.10 Source Trace Domain (inside Inventory)

These models exist to support future financial valuation without embedding accounting into inventory.

### InventorySourceBalance (Aggregate Root)
Represents the remaining balance of an inbound source.

Fields:
- SourceBalanceId
- BusinessId
- CreatedAt
- UpdatedAt
- CreatedBy
- UpdatedBy
- RowVersion
- IsDeleted

- SourceType(Receipt|ReturnRestock|AdjustmentIncrease|OpeningBalance)
- SourceDocumentRef
- SourceDocumentLineRef
- SourceTransactionRef
- SourceTransactionLineRef

- VariantRef
- SellerRef
- WarehouseRef
- LocationRef
- QualityStatusRef
- LotBatchNo
- SerialRef

- ReceivedQty
- AllocatedQty
- ConsumedQty
- AvailableQty
- RemainingQty
- BaseUomRef

- Status(Open|Consumed|Closed|Cancelled)
- OpenedAt
- ClosedAt
- LastConsumedAt

Meaning:
Tracks the open receipt/inbound balance available for reservation allocation and outbound consumption tracing.

---

### InventorySourceAllocation (Entity)
Bridges reservation allocation to a source balance.

Fields:
- AllocationId
- SourceBalanceRef
- ReservationRef
- ReservationAllocationRef
- VariantRef
- AllocatedQty
- BaseUomRef
- CreatedAt
- CreatedBy

Meaning:
Says which part of a reservation is backed by which inbound source.

---

### InventorySourceConsumption (Entity)
Child entity under outbound transaction line.

Fields:
- ConsumptionId
- OutboundTransactionRef
- OutboundTransactionLineRef
- SourceBalanceRef
- SourceDocumentRef
- SourceDocumentLineRef
- SourceTransactionRef
- SourceTransactionLineRef
- VariantRef
- SellerRef
- WarehouseRef
- LocationRef
- QualityStatusRef
- LotBatchNo
- SerialRef
- ConsumedQty
- BaseUomRef
- CreatedAt
- CreatedBy
- ReasonCode

Meaning:
Explains which inbound source(s) an outbound transaction line actually consumed.

---

# 4. Required Invariants

## 4.1 ProductVariant
- VariantSku must be unique
- Barcode, if present, must be unique
- TrackingPolicy must be one of None|Batch|Serial
- BaseUomRef is required
- TrackingPolicy and BaseUomRef must be immutable or tightly controlled after first inventory movement
- If TrackingPolicy = None:
  - SerialItem creation is not allowed
  - LotBatchNo must not be mandatory
- If TrackingPolicy = Batch:
  - LotBatchNo is required for relevant inventory operations
- If TrackingPolicy = Serial:
  - Every moved unit must be serial-traceable

## 4.2 StockDetail
- Bucket key must be unique
- QuantityOnHand must not go negative unless explicitly allowed by business rule
- Batch-tracked items require LotBatchNo
- LocationRef must belong to WarehouseRef
- QuantityOnHand may only change through a valid posted InventoryTransaction
- FirstReceivedAt must not be after LastReceivedAt

## 4.3 SerialItem
- SerialNo + VariantRef must be unique
- SerialItem not allowed for non-serial-tracked variants
- A serial can only have one valid warehouse/location/quality/status at a time
- If status = Issued, it must not count as available stock
- If status = Reserved, it must not be allocated to another active reservation
- StockDetailRef dimensions must match serial dimensions
- LastTransactionRef must point to the latest valid movement

## 4.4 InventoryTransaction
- TransactionNo must be unique
- Only allowed transaction types may be used
- Only Posted transactions affect stock
- Posted transactions must be immutable
- Cancelled or Reversed transactions must not be posted again
- Valid state transitions:
  - Draft -> Posted
  - Draft -> Cancelled
  - Posted -> Reversed
- Reversed transaction requires ReversedTransactionRef
- Sensitive operations require CorrelationId and IdempotencyKey
- Transaction must have at least one valid line

## 4.5 InventoryTransactionLine
- BaseQtyDelta must not be zero
- BaseUomRef is required
- If InputQty exists, InputUomRef must exist
- Transfer requires source and destination locations, and they must differ
- Issue requires source location
- Receipt requires destination resolution
- Batch-tracked items require LotBatchNo
- Serial-tracked items require serial detail
- For serial-tracked items, quantity must match serial count
- StockDetailRef must match line dimensions

## 4.6 InventoryDocument
- DocumentNo must be unique
- Allowed types only
- Document must have at least one valid line before approval/posting
- Rejected or Cancelled document must never be posted
- Posted document must not be posted again
- Approved-only posting if business rule requires approval
- Posted state requires PostedAt, PostedBy, PostedTransactionRef
- Approved state requires ApprovedAt, ApprovedBy
- Posting must be idempotent
- Posted document lines must become immutable

## 4.7 InventoryDocumentLine
- Qty > 0
- UomRef required
- BaseQty and BaseUomRef must be valid
- Batch-tracked line requires LotBatchNo
- Serial-tracked line requires serial detail
- Transfer requires valid distinct source/destination
- Adjustment requires ReasonCode and AdjustmentDirection
- Receipt requires destination resolution
- Issue requires source resolution
- QualityChange requires FromQualityStatusRef and ToQualityStatusRef and they must differ

## 4.8 InventoryReservation
- RequestedQuantity > 0
- AllocatedQuantity >= 0
- ConsumedQuantity >= 0
- AllocatedQuantity <= RequestedQuantity
- ConsumedQuantity <= AllocatedQuantity
- Valid transitions:
  - Pending -> Confirmed
  - Pending -> Rejected
  - Confirmed -> Consumed
  - Confirmed -> Released
  - Confirmed -> Expired
- Rejected/Released/Expired reservations must not be consumed
- Consumed reservation must not be released or expired
- Confirmed reservation requires valid allocation unless soft-reservation policy exists
- Expired reservation must not remain usable
- Reservation must link to OrderRef, OrderItemRef, VariantRef, SellerRef, WarehouseRef
- Idempotency must be enforced on create/confirm/consume/release

## 4.9 ReservationAllocation
- AllocatedQty > 0
- ReleasedQty >= 0
- ConsumedQty >= 0
- ReleasedQty <= AllocatedQty
- ConsumedQty <= AllocatedQty
- Sum of allocations for a reservation must not exceed requested quantity
- Serial must not be actively allocated twice
- StockDetailRef must be valid and dimension-compatible
- Allocation must not exceed stock unless soft allocation policy explicitly exists

## 4.10 Fulfillment
- Must link to a valid order
- WarehouseRef required
- Valid transitions:
  - Pending -> Picked
  - Picked -> Packed
  - Packed -> Shipped
  - Shipped -> Returned or PartiallyReturned
- Shipped fulfillment must not move back to earlier states except through formal workflow
- PickedAt required when Picked
- PackedAt required when Packed
- ShippedAt required when Shipped
- Returned state only valid after shipment

## 4.11 FulfillmentLine
- Qty > 0
- UomRef and BaseUomRef required
- If linked to reservation allocation, qty must not exceed consumable allocation
- Serial-tracked lines require serial detail
- PickedAt/PackedAt/ShippedAt required according to line state
- Returned line must have been previously shipped

## 4.12 ReturnRequest
- Must link to valid order or order item
- Valid transitions:
  - Requested -> Approved
  - Requested -> Rejected
  - Approved -> Received
  - Received -> Closed
- Rejected return must not become received
- Closed return must not reopen except through formal workflow
- Approved state requires ApprovedAt and ApprovedBy
- Rejected state requires RejectedAt and RejectedBy
- Received state requires ReceivedAt and ReceivedBy
- WarehouseRef must be valid

## 4.13 ReturnLine
- Qty > 0
- UomRef and BaseUomRef required
- Return quantity must not exceed shipped quantity
- Serial-tracked returns require serial detail
- Disposition must be Restock|Quarantine|Scrap
- ReceivedCondition only valid at or after receive stage
- Restock disposition must allow conversion into inventory posting

## 4.14 InventorySourceBalance
- ReceivedQty > 0
- AllocatedQty >= 0
- ConsumedQty >= 0
- AvailableQty >= 0
- RemainingQty >= 0
- AvailableQty = ReceivedQty - AllocatedQty
- RemainingQty = ReceivedQty - ConsumedQty
- AllocatedQty <= ReceivedQty
- ConsumedQty <= ReceivedQty
- Source line identity must be unique
- VariantRef and BaseUomRef required
- LocationRef must belong to WarehouseRef
- Batch-tracked source requires LotBatchNo
- Serial-tracked source requires SerialRef
- If RemainingQty > 0, status must be Open
- If status is Consumed or Closed, RemainingQty must be zero

## 4.15 InventorySourceAllocation
- AllocatedQty > 0
- SourceBalanceRef required
- ReservationRef or ReservationAllocationRef must be valid
- AllocatedQty must not exceed SourceBalance.AvailableQty
- Variant must match reservation and source balance
- Lot/serial compatibility must be respected
- Serial source must not be actively allocated twice

## 4.16 InventorySourceConsumption
- ConsumedQty > 0
- OutboundTransactionLineRef required
- SourceBalanceRef required
- Variant must match outbound line and source balance
- Lot/serial compatibility must be respected
- Sum of source consumptions for an outbound line must equal outbound quantity
- Consumption must not exceed SourceBalance.RemainingQty
- Duplicate serial consumption must not occur

---

# 5. Cross-Aggregate Rules

- Every posted InventoryDocument must link to exactly one valid InventoryTransaction
- Every posted InventoryTransaction must have fully applied stock effect
- Active reservations must not exceed reservable availability unless over-reservation policy exists
- A serial cannot be Available and simultaneously reserved for two different active reservations
- Fulfillment line must not consume more than the reservation allocation supports
- Return receive flow must not occur for items never shipped
- Every source allocation must point to an open valid source balance
- Total source consumptions must not over-consume any source balance
- Outbound posted line requiring valuation trace must have valid source consumptions
- Inventory must be able to explain which inbound source lines an outbound line consumed for future financial valuation

---

# 6. Source Trace Rules for Financial Integration

This service must support receipt-based financial valuation without embedding accounting logic.

## 6.1 Inbound
When an inbound source is posted:
- stock is increased
- InventorySourceBalance is opened

## 6.2 Reservation
When reservation allocation occurs:
- InventorySourceAllocation may bind reservation allocation to source balances

## 6.3 Outbound
When issue/reserve-consume is posted:
- InventorySourceConsumption must be created
- InventorySourceBalance must be updated

## 6.4 Financial integration expectation
Financial service should later be able to determine:
- which receipt line had valuation
- which outbound line consumed which receipt source
- how much quantity of each source was consumed

This service must not calculate accounting entries itself.

---

# 7. Standard Flows

## 7.1 Direct Warehouse Operations
Used for:
- Receipt
- Manual Issue
- Transfer
- Adjustment
- QualityChange
- Return stock posting

Flow:
`InventoryDocument -> InventoryTransaction -> StockDetail`

## 7.2 Order-Driven Outbound
Flow:
`Order -> Reservation -> Fulfillment -> InventoryDocument -> InventoryTransaction -> StockDetail`

Behavior:
- reservation holds availability
- fulfillment executes warehouse work
- shipping triggers issue posting
- stock changes only at posting

## 7.3 Source Trace Flow
Inbound:
`Receipt Posted -> InventorySourceBalance created`

Reservation:
`Reservation -> InventorySourceAllocation created`

Outbound:
`Issue/ReserveConsume Posted -> InventorySourceConsumption created -> InventorySourceBalance updated`

---

# 8. Command Set

## 8.1 Document Commands
- CreateReceiptDocumentCommand
- CreateIssueDocumentCommand
- CreateTransferDocumentCommand
- CreateAdjustmentDocumentCommand
- CreateReturnDocumentCommand
- CreateQualityChangeDocumentCommand
- ApproveInventoryDocumentCommand
- RejectInventoryDocumentCommand
- CancelInventoryDocumentCommand
- PostInventoryDocumentCommand

## 8.2 Reservation Commands
- CreateReservationCommand
- ConfirmReservationCommand
- ReleaseReservationCommand
- ExpireReservationCommand
- ConsumeReservationCommand

## 8.3 Fulfillment Commands
- CreateFulfillmentCommand
- MarkFulfillmentPickedCommand
- MarkFulfillmentPackedCommand
- MarkFulfillmentShippedCommand

## 8.4 Return Commands
- CreateReturnRequestCommand
- ApproveReturnRequestCommand
- RejectReturnRequestCommand
- ReceiveReturnRequestCommand
- CloseReturnRequestCommand

---

# 9. Handler and Domain Logic Guidance

## Aggregate methods own:
- internal invariants
- state transitions
- quantity guards
- immutability rules

## Application handlers/services own:
- orchestration across multiple aggregates
- loading required aggregates
- cross-aggregate validation
- posting workflow coordination
- source allocation and source consumption resolution
- next-step workflow progression

## Suggested policy/services:
- InventoryAvailabilityService
- ReservationAllocationPolicy
- SourceAllocationResolver
- SourceConsumptionResolver
- InventoryPostingService

---

# 10. Important Implementation Notes

- Use optimistic concurrency with RowVersion
- Enforce idempotency for all sensitive posting operations
- Posted transactions must be immutable
- Projections must be rebuildable from source truth
- Fulfillment must never mutate stock directly
- Reservation must never mutate physical stock directly
- Source tracing must be complete enough for financial valuation later
- Keep accounting logic out of this service

---

# 11. Immediate Development Focus

For the first implementation phase, focus on:

1. InventoryDocument
2. InventoryTransaction
3. StockDetail
4. Reservation
5. Fulfillment
6. Return core
7. InventorySourceBalance
8. InventorySourceAllocation
9. InventorySourceConsumption

Do not expand into procurement, accounting, shipping, or advanced WMS yet.

---

# 12. Final Design Summary

This microservice is an **Inventory Core** with:

- strong stock truth
- official inventory document workflow
- immutable movement journal
- reservation support
- fulfillment execution handoff
- return handling
- source tracing for future valuation

It is intentionally designed so that a future Financial service can use receipt-based source tracing to compute valuation and COGS without forcing accounting behavior into inventory.
