namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record AttributeDefinitionOptionAddedEvent : IDomainEvent
{
    public BusinessKey AttributeDefinitionBusinessKey { get; }
    public Guid OptionBusinessKey { get; }
    public string Name { get; }
    public string Value { get; }
    public int DisplayOrder { get; }
    public bool IsActive { get; }
    public DateTime OccurredOn { get; }

    public AttributeDefinitionOptionAddedEvent(
        BusinessKey attributeDefinitionBusinessKey,
        Guid optionBusinessKey,
        string value,
        int displayOrder,
        bool isActive,
        string? name = null)
    {
        AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        OptionBusinessKey = optionBusinessKey;
        Value = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value is required.", nameof(value))
            : value.Trim();
        Name = string.IsNullOrWhiteSpace(name) ? Value : name.Trim();
        DisplayOrder = displayOrder;
        IsActive = isActive;
        OccurredOn = DateTime.UtcNow;
    }
}
