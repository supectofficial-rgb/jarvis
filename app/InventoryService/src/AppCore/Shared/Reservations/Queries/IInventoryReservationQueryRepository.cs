namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public interface IInventoryReservationQueryRepository : IQueryRepository
{
    Task<GetInventoryReservationByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid reservationBusinessKey);
    Task<ReservationListItem?> GetByIdAsync(Guid reservationId);
    Task<List<ReservationListItem>> GetByOrderAsync(Guid orderRef);
    Task<List<ReservationListItem>> GetByVariantAsync(Guid variantRef);
    Task<List<ReservationListItem>> GetActiveAsync();
    Task<ReservationListItem?> GetSummaryAsync(Guid reservationBusinessKey);
    Task<List<ReservationAllocationListItem>> GetAllocationsByReservationIdAsync(Guid reservationBusinessKey);
    Task<List<ReservationAllocationListItem>> GetAllocationsByStockDetailIdAsync(Guid stockDetailBusinessKey);
}
