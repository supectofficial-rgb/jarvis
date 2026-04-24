namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public interface IInventorySourceBalanceQueryRepository : IQueryRepository
{
    Task<GetInventorySourceBalanceByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid sourceBalanceBusinessKey);
    Task<InventorySourceBalanceListItem?> GetByIdAsync(Guid sourceBalanceId);
    Task<List<InventorySourceBalanceListItem>> GetOpenByVariantAsync(Guid variantRef);
    Task<InventorySourceBalanceSummaryItem?> GetSummaryAsync(Guid sourceBalanceBusinessKey);
    Task<List<InventorySourceAllocationListItem>> GetAllocationsByReservationIdAsync(Guid reservationRef);
    Task<List<InventorySourceAllocationListItem>> GetAllocationsBySourceBalanceIdAsync(Guid sourceBalanceBusinessKey);
    Task<List<InventorySourceConsumptionListItem>> GetConsumptionsByTransactionLineAsync(Guid outboundTransactionLineRef);
    Task<List<InventorySourceConsumptionListItem>> GetConsumptionsBySourceBalanceIdAsync(Guid sourceBalanceBusinessKey);
}
