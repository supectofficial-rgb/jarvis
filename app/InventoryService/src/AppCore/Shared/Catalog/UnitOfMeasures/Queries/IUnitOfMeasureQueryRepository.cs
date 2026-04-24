namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.SearchUnitOfMeasures;
using OysterFx.AppCore.Shared.Queries;

public interface IUnitOfMeasureQueryRepository : IQueryRepository
{
    Task<GetUnitOfMeasureByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid unitOfMeasureBusinessKey);
    Task<GetUnitOfMeasureByBusinessKeyQueryResult?> GetByIdAsync(Guid unitOfMeasureId);
    Task<GetUnitOfMeasureByBusinessKeyQueryResult?> GetByCodeAsync(string code);
    Task<SearchUnitOfMeasuresQueryResult> SearchAsync(SearchUnitOfMeasuresQuery query);
    Task<List<UnitOfMeasureListItem>> GetActiveUnitOfMeasuresAsync();
    Task<List<UnitOfMeasureLookupItem>> GetLookupAsync(bool includeInactive = false);
}
