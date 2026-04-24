namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries;

using Insurance.InventoryService.AppCore.Shared.Returns.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public interface IReturnRequestQueryRepository : IQueryRepository
{
    Task<GetReturnRequestByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid returnRequestBusinessKey);
    Task<ReturnRequestListItem?> GetByIdAsync(Guid returnRequestId);
    Task<List<ReturnRequestListItem>> GetByOrderAsync(Guid orderRef);
    Task<List<ReturnRequestListItem>> GetByStatusAsync(string status);
    Task<ReturnRequestListItem?> GetSummaryAsync(Guid returnRequestBusinessKey);
}
