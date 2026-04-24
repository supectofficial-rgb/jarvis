namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.AllocateInventorySourceBalance;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.CloseInventorySourceBalance;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.ConsumeInventorySourceBalance;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.CreateInventorySourceAllocation;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.CreateInventorySourceConsumption;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.OpenInventorySourceBalance;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.ReleaseInventorySourceAllocation;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetAllocationsByReservationId;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetAllocationsBySourceBalanceId;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetConsumptionsBySourceBalanceId;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetConsumptionsByTransactionLine;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetOpenByVariant;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetSummary;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class SourceBalanceController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Open([FromBody] OpenInventorySourceBalanceCommand command)
        => SendCommand<OpenInventorySourceBalanceCommand, Guid>(command);

    [HttpPost("{sourceBalanceBusinessKey:guid}/allocate")]
    public Task<IActionResult> Allocate([FromRoute] Guid sourceBalanceBusinessKey, [FromBody] AllocateInventorySourceBalanceCommand command)
    {
        command.SourceBalanceBusinessKey = sourceBalanceBusinessKey;
        return SendCommand<AllocateInventorySourceBalanceCommand, Guid>(command);
    }

    [HttpPost("{sourceBalanceBusinessKey:guid}/consume")]
    public Task<IActionResult> Consume([FromRoute] Guid sourceBalanceBusinessKey, [FromBody] ConsumeInventorySourceBalanceCommand command)
    {
        command.SourceBalanceBusinessKey = sourceBalanceBusinessKey;
        return SendCommand<ConsumeInventorySourceBalanceCommand, Guid>(command);
    }

    [HttpPost("{sourceBalanceBusinessKey:guid}/close")]
    public Task<IActionResult> Close([FromRoute] Guid sourceBalanceBusinessKey, [FromBody] CloseInventorySourceBalanceCommand? command)
    {
        var request = command ?? new CloseInventorySourceBalanceCommand();
        request.SourceBalanceBusinessKey = sourceBalanceBusinessKey;
        return SendCommand<CloseInventorySourceBalanceCommand, Guid>(request);
    }

    [HttpPost("{sourceBalanceBusinessKey:guid}/source-allocations")]
    public Task<IActionResult> CreateAllocation([FromRoute] Guid sourceBalanceBusinessKey, [FromBody] CreateInventorySourceAllocationCommand command)
    {
        command.SourceBalanceBusinessKey = sourceBalanceBusinessKey;
        return SendCommand<CreateInventorySourceAllocationCommand, Guid>(command);
    }

    [HttpPost("{sourceBalanceBusinessKey:guid}/source-allocations/{allocationBusinessKey:guid}/release")]
    public Task<IActionResult> ReleaseAllocation([FromRoute] Guid sourceBalanceBusinessKey, [FromRoute] Guid allocationBusinessKey, [FromBody] ReleaseInventorySourceAllocationCommand command)
    {
        command.SourceBalanceBusinessKey = sourceBalanceBusinessKey;
        command.AllocationBusinessKey = allocationBusinessKey;
        return SendCommand<ReleaseInventorySourceAllocationCommand, Guid>(command);
    }

    [HttpPost("{sourceBalanceBusinessKey:guid}/source-consumptions")]
    public Task<IActionResult> CreateConsumption([FromRoute] Guid sourceBalanceBusinessKey, [FromBody] CreateInventorySourceConsumptionCommand command)
    {
        command.SourceBalanceBusinessKey = sourceBalanceBusinessKey;
        return SendCommand<CreateInventorySourceConsumptionCommand, Guid>(command);
    }

    [HttpGet("{sourceBalanceBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid sourceBalanceBusinessKey)
        => ExecuteQueryAsync<GetInventorySourceBalanceByBusinessKeyQuery, GetInventorySourceBalanceByBusinessKeyQueryResult>(
            new GetInventorySourceBalanceByBusinessKeyQuery(sourceBalanceBusinessKey));

    [HttpGet("by-id/{sourceBalanceId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid sourceBalanceId)
        => ExecuteQueryAsync<GetInventorySourceBalanceByIdQuery, GetInventorySourceBalanceByIdQueryResult>(
            new GetInventorySourceBalanceByIdQuery(sourceBalanceId));

    [HttpGet("open/by-variant/{variantRef:guid}")]
    public Task<IActionResult> GetOpenByVariant([FromRoute] Guid variantRef)
        => ExecuteQueryAsync<GetOpenInventorySourceBalancesByVariantQuery, GetOpenInventorySourceBalancesByVariantQueryResult>(
            new GetOpenInventorySourceBalancesByVariantQuery(variantRef));

    [HttpGet("{sourceBalanceBusinessKey:guid}/summary")]
    public Task<IActionResult> GetSummary([FromRoute] Guid sourceBalanceBusinessKey)
        => ExecuteQueryAsync<GetInventorySourceBalanceSummaryQuery, GetInventorySourceBalanceSummaryQueryResult>(
            new GetInventorySourceBalanceSummaryQuery(sourceBalanceBusinessKey));

    [HttpGet("source-allocations/by-reservation/{reservationRef:guid}")]
    public Task<IActionResult> GetAllocationsByReservation([FromRoute] Guid reservationRef)
        => ExecuteQueryAsync<GetInventorySourceAllocationsByReservationIdQuery, GetInventorySourceAllocationsByReservationIdQueryResult>(
            new GetInventorySourceAllocationsByReservationIdQuery(reservationRef));

    [HttpGet("{sourceBalanceBusinessKey:guid}/source-allocations")]
    public Task<IActionResult> GetAllocationsBySourceBalance([FromRoute] Guid sourceBalanceBusinessKey)
        => ExecuteQueryAsync<GetInventorySourceAllocationsBySourceBalanceIdQuery, GetInventorySourceAllocationsBySourceBalanceIdQueryResult>(
            new GetInventorySourceAllocationsBySourceBalanceIdQuery(sourceBalanceBusinessKey));

    [HttpGet("source-consumptions/by-transaction-line/{outboundTransactionLineRef:guid}")]
    public Task<IActionResult> GetConsumptionsByTransactionLine([FromRoute] Guid outboundTransactionLineRef)
        => ExecuteQueryAsync<GetInventorySourceConsumptionsByTransactionLineQuery, GetInventorySourceConsumptionsByTransactionLineQueryResult>(
            new GetInventorySourceConsumptionsByTransactionLineQuery(outboundTransactionLineRef));

    [HttpGet("{sourceBalanceBusinessKey:guid}/source-consumptions")]
    public Task<IActionResult> GetConsumptionsBySourceBalance([FromRoute] Guid sourceBalanceBusinessKey)
        => ExecuteQueryAsync<GetInventorySourceConsumptionsBySourceBalanceIdQuery, GetInventorySourceConsumptionsBySourceBalanceIdQueryResult>(
            new GetInventorySourceConsumptionsBySourceBalanceIdQuery(sourceBalanceBusinessKey));
}
