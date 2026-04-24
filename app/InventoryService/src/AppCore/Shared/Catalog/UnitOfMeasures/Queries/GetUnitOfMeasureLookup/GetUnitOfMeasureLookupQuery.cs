namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetUnitOfMeasureLookup;

using OysterFx.AppCore.Shared.Queries;

public class GetUnitOfMeasureLookupQuery : IQuery<GetUnitOfMeasureLookupQueryResult>
{
    public bool IncludeInactive { get; set; }
}
