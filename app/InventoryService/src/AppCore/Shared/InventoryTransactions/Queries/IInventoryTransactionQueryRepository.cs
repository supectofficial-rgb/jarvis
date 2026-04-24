namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.SearchInventoryTransactions;
using OysterFx.AppCore.Shared.Queries;

public interface IInventoryTransactionQueryRepository : IQueryRepository
{
    Task<GetInventoryTransactionByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid transactionBusinessKey);
    Task<InventoryTransactionListItem?> GetByIdAsync(Guid transactionId);
    Task<InventoryTransactionListItem?> GetByNoAsync(string transactionNo);
    Task<SearchInventoryTransactionsQueryResult> SearchAsync(SearchInventoryTransactionsQuery query);
    Task<List<InventoryTransactionListItem>> GetByVariantAsync(Guid variantRef);
    Task<List<InventoryTransactionListItem>> GetByWarehouseAsync(Guid warehouseRef);
}
