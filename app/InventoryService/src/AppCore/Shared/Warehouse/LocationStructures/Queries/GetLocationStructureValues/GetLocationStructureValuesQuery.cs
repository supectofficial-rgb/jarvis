namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetLocationStructureValues;

using OysterFx.AppCore.Shared.Queries;

public sealed class GetLocationStructureValuesQuery : IQuery<GetLocationStructureValuesQueryResult>
{
    public GetLocationStructureValuesQuery(Guid structureRef, bool includeInactive = false, string? searchTerm = null)
    {
        StructureRef = structureRef;
        IncludeInactive = includeInactive;
        SearchTerm = searchTerm;
    }

    public Guid StructureRef { get; }
    public bool IncludeInactive { get; }
    public string? SearchTerm { get; }
}
