namespace Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record InventoryTransactionPostedEvent(BusinessKey TransactionBusinessKey, DateTime OccurredOn) : IDomainEvent;
