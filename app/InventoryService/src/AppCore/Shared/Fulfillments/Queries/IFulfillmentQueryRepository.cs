namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public interface IFulfillmentQueryRepository : IQueryRepository
{
    Task<GetFulfillmentByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid fulfillmentBusinessKey);
    Task<FulfillmentListItem?> GetByIdAsync(Guid fulfillmentId);
    Task<List<FulfillmentListItem>> GetByOrderAsync(Guid orderRef);
    Task<List<FulfillmentListItem>> GetByStatusAsync(string status);
    Task<FulfillmentListItem?> GetSummaryAsync(Guid fulfillmentBusinessKey);
}
