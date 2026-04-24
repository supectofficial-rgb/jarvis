namespace Insurance.InventoryService.AppCore.Domain.Reservations.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ReservationAllocationConsumedEvent(BusinessKey ReservationAllocationBusinessKey, DateTime OccurredOn) : IDomainEvent;
