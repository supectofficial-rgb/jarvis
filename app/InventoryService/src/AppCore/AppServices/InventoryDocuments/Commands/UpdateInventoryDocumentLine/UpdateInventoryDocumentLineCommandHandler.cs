namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.UpdateInventoryDocumentLine;

using Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;
using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.UpdateInventoryDocumentLine;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class UpdateInventoryDocumentLineCommandHandler : CommandHandler<UpdateInventoryDocumentLineCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;
    private readonly IInventorySourceBalanceCommandRepository _sourceBalanceRepository;
    private readonly ISerialItemCommandRepository _serialItemCommandRepository;
    private readonly ISerialItemQueryRepository _serialItemQueryRepository;
    private readonly ILogger<UpdateInventoryDocumentLineCommandHandler> _logger;

    public UpdateInventoryDocumentLineCommandHandler(
        IInventoryDocumentCommandRepository repository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository,
        ISerialItemCommandRepository serialItemCommandRepository,
        ISerialItemQueryRepository serialItemQueryRepository,
        ILogger<UpdateInventoryDocumentLineCommandHandler> logger)
    {
        _repository = repository;
        _sourceBalanceRepository = sourceBalanceRepository;
        _serialItemCommandRepository = serialItemCommandRepository;
        _serialItemQueryRepository = serialItemQueryRepository;
        _logger = logger;
    }

    public override async Task<CommandResult<Guid>> Handle(UpdateInventoryDocumentLineCommand command)
    {
        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
            return Fail("Document not found.");

        if (document.Status != InventoryDocumentStatus.Draft)
            return Fail("Only draft documents can be modified.");

        try
        {
            var existingLine = document.Lines.FirstOrDefault(x => x.BusinessKey.Value == command.LineBusinessKey);
            if (existingLine is null)
                return Fail("Document line was not found.");

            var previousSerials = existingLine.Serials
                .Select(serial => (serial.SerialRef, serial.SerialNo))
                .ToList();

            Enum.TryParse<InventoryAdjustmentDirection>(command.Line.AdjustmentDirection, true, out var adjustmentDirection);
            var serials = command.Line.Serials
                .Select(x => (x.SerialRef, x.SerialNo))
                .ToList();

            document.UpdateLine(
                command.LineBusinessKey,
                command.Line.VariantRef,
                command.Line.Qty,
                command.Line.UomRef,
                command.Line.BaseQty,
                command.Line.BaseUomRef,
                command.Line.SourceLocationRef,
                command.Line.DestinationLocationRef,
                command.Line.QualityStatusRef,
                command.Line.FromQualityStatusRef,
                command.Line.ToQualityStatusRef,
                command.Line.LotBatchNo,
                command.Line.ReasonCode,
                command.Line.AdjustmentDirection is null ? null : adjustmentDirection,
                serials);

            if (document.DocumentType == InventoryDocumentType.Receipt)
            {
                InventoryDocumentReceiptLotHelper.ApplyReceiptLotBatchNo(document);
            }

            if (InventoryDocumentLineSerialStatusHelper.ShouldReserveSerials(document.DocumentType))
            {
                var previousSerialItems = await InventoryDocumentLineSerialStatusHelper.ResolveSerialItemsAsync(
                    previousSerials,
                    command.Line.VariantRef,
                    _serialItemCommandRepository,
                    _serialItemQueryRepository);
                InventoryDocumentLineSerialStatusHelper.ReleaseSerialItems(previousSerialItems);

                var newSerialItems = await InventoryDocumentLineSerialStatusHelper.ResolveSerialItemsAsync(
                    serials,
                    command.Line.VariantRef,
                    _serialItemCommandRepository,
                    _serialItemQueryRepository);
                InventoryDocumentLineSerialStatusHelper.ReserveSerialItems(newSerialItems);
            }

            if (InventoryDocumentLineSourceAllocationHelper.ShouldReserveSourceBalances(document.DocumentType))
            {
                await InventoryDocumentLineSourceAllocationHelper.ReleaseSourceAllocationsAsync(existingLine.BusinessKey.Value, _sourceBalanceRepository);
                await InventoryDocumentLineSourceAllocationHelper.AllocateSourceBalancesAsync(document, existingLine, _sourceBalanceRepository, _logger);
            }
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(command.LineBusinessKey);
    }
}
