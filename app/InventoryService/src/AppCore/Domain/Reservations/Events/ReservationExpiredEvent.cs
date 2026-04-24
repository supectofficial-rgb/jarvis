namespace Insurance.InventoryService.AppCore.Domain.Reservations.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ReservationExpiredEvent(BusinessKey ReservationBusinessKey, DateTime OccurredOn) : IDomainEvent;
