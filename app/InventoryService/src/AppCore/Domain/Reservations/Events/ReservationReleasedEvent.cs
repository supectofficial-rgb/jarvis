namespace Insurance.InventoryService.AppCore.Domain.Reservations.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ReservationReleasedEvent(BusinessKey ReservationBusinessKey, DateTime OccurredOn) : IDomainEvent;
