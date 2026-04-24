namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands.AllocateReservation;

using OysterFx.AppCore.Shared.Commands;

public class AllocateReservationCommand : ICommand<Guid>
{
    public Guid ReservationBusinessKey { get; set; }
    public Guid? StockDetailRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public decimal AllocatedQty { get; set; }
    public string? LotBatchNo { get; set; }
    public Guid? SerialRef { get; set; }
}
