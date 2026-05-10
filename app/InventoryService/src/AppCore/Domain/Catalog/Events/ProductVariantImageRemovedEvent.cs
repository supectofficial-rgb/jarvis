namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantImageRemovedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public string FileKey { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantImageRemovedEvent(BusinessKey productVariantBusinessKey, string fileKey)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        FileKey = fileKey;
        OccurredOn = DateTime.UtcNow;
    }
}
