namespace Insurance.InventoryService.AppCore.Domain.Returns.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ReturnRequestRejectedEvent(BusinessKey ReturnRequestBusinessKey, DateTime OccurredOn) : IDomainEvent;
