namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetQualityStatusLookup;

using OysterFx.AppCore.Shared.Queries;

public class GetQualityStatusLookupQuery : IQuery<GetQualityStatusLookupQueryResult>
{
    public bool IncludeInactive { get; set; }
}
