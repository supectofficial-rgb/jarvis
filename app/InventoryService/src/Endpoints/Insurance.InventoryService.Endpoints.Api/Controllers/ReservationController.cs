namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ConfirmReservation;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ConsumeReservation;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ConsumeReservationAllocation;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.CreateReservation;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.AllocateReservation;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ExpireReservation;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.RejectReservation;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ReleaseReservation;
using Insurance.InventoryService.AppCore.Shared.Reservations.Commands.ReleaseReservationAllocation;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByOrder;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByVariant;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetActiveReservations;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationAllocationsByReservationId;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationAllocationsByStockDetailId;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationSummary;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class ReservationController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateReservationCommand command)
        => SendCommand<CreateReservationCommand, Guid>(command);

    [HttpPost("{reservationBusinessKey:guid}/confirm")]
    public Task<IActionResult> Confirm([FromRoute] Guid reservationBusinessKey, [FromBody] ConfirmReservationCommand? command)
    {
        var request = command ?? new ConfirmReservationCommand();
        request.ReservationBusinessKey = reservationBusinessKey;
        return SendCommand<ConfirmReservationCommand, Guid>(request);
    }

    [HttpPost("{reservationBusinessKey:guid}/consume")]
    public Task<IActionResult> Consume([FromRoute] Guid reservationBusinessKey, [FromBody] ConsumeReservationCommand command)
    {
        command.ReservationBusinessKey = reservationBusinessKey;
        return SendCommand<ConsumeReservationCommand, Guid>(command);
    }

    [HttpPost("{reservationBusinessKey:guid}/release")]
    public Task<IActionResult> Release([FromRoute] Guid reservationBusinessKey, [FromBody] ReleaseReservationCommand? command)
    {
        var request = command ?? new ReleaseReservationCommand();
        request.ReservationBusinessKey = reservationBusinessKey;
        return SendCommand<ReleaseReservationCommand, Guid>(request);
    }

    [HttpPost("{reservationBusinessKey:guid}/expire")]
    public Task<IActionResult> Expire([FromRoute] Guid reservationBusinessKey, [FromBody] ExpireReservationCommand? command)
    {
        var request = command ?? new ExpireReservationCommand();
        request.ReservationBusinessKey = reservationBusinessKey;
        return SendCommand<ExpireReservationCommand, Guid>(request);
    }

    [HttpPost("{reservationBusinessKey:guid}/reject")]
    public Task<IActionResult> Reject([FromRoute] Guid reservationBusinessKey, [FromBody] RejectReservationCommand? command)
    {
        var request = command ?? new RejectReservationCommand();
        request.ReservationBusinessKey = reservationBusinessKey;
        return SendCommand<RejectReservationCommand, Guid>(request);
    }

    [HttpPost("{reservationBusinessKey:guid}/allocations")]
    public Task<IActionResult> Allocate([FromRoute] Guid reservationBusinessKey, [FromBody] AllocateReservationCommand command)
    {
        command.ReservationBusinessKey = reservationBusinessKey;
        return SendCommand<AllocateReservationCommand, Guid>(command);
    }

    [HttpPost("{reservationBusinessKey:guid}/allocations/{allocationBusinessKey:guid}/release")]
    public Task<IActionResult> ReleaseAllocation([FromRoute] Guid reservationBusinessKey, [FromRoute] Guid allocationBusinessKey, [FromBody] ReleaseReservationAllocationCommand command)
    {
        command.ReservationBusinessKey = reservationBusinessKey;
        command.AllocationBusinessKey = allocationBusinessKey;
        return SendCommand<ReleaseReservationAllocationCommand, Guid>(command);
    }

    [HttpPost("{reservationBusinessKey:guid}/allocations/{allocationBusinessKey:guid}/consume")]
    public Task<IActionResult> ConsumeAllocation([FromRoute] Guid reservationBusinessKey, [FromRoute] Guid allocationBusinessKey, [FromBody] ConsumeReservationAllocationCommand command)
    {
        command.ReservationBusinessKey = reservationBusinessKey;
        command.AllocationBusinessKey = allocationBusinessKey;
        return SendCommand<ConsumeReservationAllocationCommand, Guid>(command);
    }

    [HttpGet("{reservationBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid reservationBusinessKey)
        => ExecuteQueryAsync<GetInventoryReservationByBusinessKeyQuery, GetInventoryReservationByBusinessKeyQueryResult>(
            new GetInventoryReservationByBusinessKeyQuery(reservationBusinessKey));

    [HttpGet("by-id/{reservationId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid reservationId)
        => ExecuteQueryAsync<GetReservationByIdQuery, GetReservationByIdQueryResult>(
            new GetReservationByIdQuery(reservationId));

    [HttpGet("by-order/{orderRef:guid}")]
    public Task<IActionResult> GetByOrder([FromRoute] Guid orderRef)
        => ExecuteQueryAsync<GetReservationsByOrderQuery, GetReservationsByOrderQueryResult>(
            new GetReservationsByOrderQuery(orderRef));

    [HttpGet("by-variant/{variantRef:guid}")]
    public Task<IActionResult> GetByVariant([FromRoute] Guid variantRef)
        => ExecuteQueryAsync<GetReservationsByVariantQuery, GetReservationsByVariantQueryResult>(
            new GetReservationsByVariantQuery(variantRef));

    [HttpGet("active")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActiveReservationsQuery, GetActiveReservationsQueryResult>(
            new GetActiveReservationsQuery());

    [HttpGet("{reservationBusinessKey:guid}/summary")]
    public Task<IActionResult> GetSummary([FromRoute] Guid reservationBusinessKey)
        => ExecuteQueryAsync<GetReservationSummaryQuery, GetReservationSummaryQueryResult>(
            new GetReservationSummaryQuery(reservationBusinessKey));

    [HttpGet("{reservationBusinessKey:guid}/allocations")]
    public Task<IActionResult> GetAllocationsByReservation([FromRoute] Guid reservationBusinessKey)
        => ExecuteQueryAsync<GetReservationAllocationsByReservationIdQuery, GetReservationAllocationsByReservationIdQueryResult>(
            new GetReservationAllocationsByReservationIdQuery(reservationBusinessKey));

    [HttpGet("allocations/by-stock-detail/{stockDetailBusinessKey:guid}")]
    public Task<IActionResult> GetAllocationsByStockDetail([FromRoute] Guid stockDetailBusinessKey)
        => ExecuteQueryAsync<GetReservationAllocationsByStockDetailIdQuery, GetReservationAllocationsByStockDetailIdQueryResult>(
            new GetReservationAllocationsByStockDetailIdQuery(stockDetailBusinessKey));
}
