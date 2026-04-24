namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.SearchQualityStatuses;
using OysterFx.AppCore.Shared.Queries;

public interface IQualityStatusQueryRepository : IQueryRepository
{
    Task<GetQualityStatusByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid qualityStatusBusinessKey);
    Task<GetQualityStatusByBusinessKeyQueryResult?> GetByIdAsync(Guid qualityStatusId);
    Task<GetQualityStatusByBusinessKeyQueryResult?> GetByCodeAsync(string code);
    Task<SearchQualityStatusesQueryResult> SearchAsync(SearchQualityStatusesQuery query);
    Task<List<QualityStatusListItem>> GetActiveQualityStatusesAsync();
    Task<List<QualityStatusLookupItem>> GetLookupAsync(bool includeInactive = false);
}
