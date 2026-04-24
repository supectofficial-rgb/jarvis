namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record UnitOfMeasureCreatedEvent : IDomainEvent
{
    public BusinessKey UnitOfMeasureBusinessKey { get; }
    public string Code { get; }
    public string Name { get; }
    public int Precision { get; }
    public bool IsActive { get; }
    public DateTime OccurredOn { get; }

    public UnitOfMeasureCreatedEvent(BusinessKey unitOfMeasureBusinessKey, string code, string name, int precision, bool isActive)
    {
        UnitOfMeasureBusinessKey = unitOfMeasureBusinessKey;
        Code = code;
        Name = name;
        Precision = precision;
        IsActive = isActive;
        OccurredOn = DateTime.UtcNow;
    }
}
