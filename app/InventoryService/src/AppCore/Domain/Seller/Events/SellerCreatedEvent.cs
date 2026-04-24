namespace Insurance.InventoryService.AppCore.Domain.Seller.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record SellerCreatedEvent(BusinessKey SellerBusinessKey, DateTime OccurredOn) : IDomainEvent;
