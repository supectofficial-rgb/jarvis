namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands;

using Insurance.InventoryService.AppCore.Domain.Reservations.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IInventoryReservationCommandRepository : ICommandRepository<InventoryReservation, long>
{
    Task<InventoryReservation?> GetByBusinessKeyAsync(Guid reservationBusinessKey);
}
