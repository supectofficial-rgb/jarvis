namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record AttributeDefinitionOptionSnapshot
{
    public Guid OptionBusinessKey { get; }
    public string Name { get; }
    public string Value { get; }
    public int DisplayOrder { get; }
    public bool IsActive { get; }

    public AttributeDefinitionOptionSnapshot(
        Guid optionBusinessKey,
        string value,
        int displayOrder,
        bool isActive,
        string? name = null)
    {
        OptionBusinessKey = optionBusinessKey;
        Value = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value is required.", nameof(value))
            : value.Trim();
        Name = string.IsNullOrWhiteSpace(name) ? Value : name.Trim();
        DisplayOrder = displayOrder;
        IsActive = isActive;
    }
}

public sealed record AttributeDefinitionCreatedEvent : IDomainEvent
{
    public BusinessKey AttributeDefinitionBusinessKey { get; }
    public string Code { get; }
    public string Name { get; }
    public AttributeDataType DataType { get; }
    public AttributeScope Scope { get; }
    public bool IsActive { get; }
    public IReadOnlyCollection<AttributeDefinitionOptionSnapshot> Options { get; }
    public DateTime OccurredOn { get; }

    public AttributeDefinitionCreatedEvent(
        BusinessKey attributeDefinitionBusinessKey,
        string code,
        string name,
        AttributeDataType dataType,
        AttributeScope scope,
        bool isActive,
        IReadOnlyCollection<AttributeDefinitionOptionSnapshot> options)
    {
        AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        Code = code;
        Name = name;
        DataType = dataType;
        Scope = scope;
        IsActive = isActive;
        Options = options;
        OccurredOn = DateTime.UtcNow;
    }
}
