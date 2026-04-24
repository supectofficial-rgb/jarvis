namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Reservations;

using Insurance.InventoryService.AppCore.Domain.Reservations.Entities;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class InventoryReservationCommandRepository
    : CommandRepository<InventoryReservation, InventoryServiceCommandDbContext>, IInventoryReservationCommandRepository
{
    public InventoryReservationCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<InventoryReservation?> GetByBusinessKeyAsync(Guid reservationBusinessKey)
    {
        return _dbContext.InventoryReservations
            .Include(x => x.Allocations)
            .Include(x => x.Transitions)
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == reservationBusinessKey);
    }
}
