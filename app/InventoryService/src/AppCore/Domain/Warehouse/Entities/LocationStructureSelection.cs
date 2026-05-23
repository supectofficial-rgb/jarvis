namespace Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class LocationStructureSelection : AggregateRoot
{
    public Guid LocationRef { get; private set; }
    public Guid StructureRef { get; private set; }
    public Guid StructureValueRef { get; private set; }

    private LocationStructureSelection()
    {
    }

    private LocationStructureSelection(Guid locationRef, Guid structureRef, Guid structureValueRef)
    {
        LocationRef = locationRef;
        StructureRef = structureRef;
        StructureValueRef = structureValueRef;
    }

    public static LocationStructureSelection Create(Guid locationRef, Guid structureRef, Guid structureValueRef)
    {
        if (locationRef == Guid.Empty)
            throw new ArgumentException("LocationRef is required.", nameof(locationRef));
        if (structureRef == Guid.Empty)
            throw new ArgumentException("StructureRef is required.", nameof(structureRef));
        if (structureValueRef == Guid.Empty)
            throw new ArgumentException("StructureValueRef is required.", nameof(structureValueRef));

        return new LocationStructureSelection(locationRef, structureRef, structureValueRef);
    }

    public void ChangeValue(Guid structureValueRef)
    {
        if (structureValueRef == Guid.Empty)
            throw new ArgumentException("StructureValueRef is required.", nameof(structureValueRef));

        StructureValueRef = structureValueRef;
    }
}
