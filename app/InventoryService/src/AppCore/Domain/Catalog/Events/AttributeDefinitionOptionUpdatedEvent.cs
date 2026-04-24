namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record AttributeDefinitionOptionUpdatedEvent : IDomainEvent
{
    public BusinessKey AttributeDefinitionBusinessKey { get; }
    public Guid OptionBusinessKey { get; }
    public string Name { get; }
    public string Value { get; }
    public int DisplayOrder { get; }
    public DateTime OccurredOn { get; }

    public AttributeDefinitionOptionUpdatedEvent(
        BusinessKey attributeDefinitionBusinessKey,
        Guid optionBusinessKey,
        string value,
        int displayOrder,
        string? name = null)
    {
        AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        OptionBusinessKey = optionBusinessKey;
        Value = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value is required.", nameof(value))
            : value.Trim();
        Name = string.IsNullOrWhiteSpace(name) ? Value : name.Trim();
        DisplayOrder = displayOrder;
        OccurredOn = DateTime.UtcNow;
    }
}
