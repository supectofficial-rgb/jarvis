namespace Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record InventoryDocumentRejectedEvent(BusinessKey DocumentBusinessKey, DateTime OccurredOn) : IDomainEvent;
