namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.AddInventoryDocumentLine;

using Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;
using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.AddInventoryDocumentLine;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class AddInventoryDocumentLineCommandHandler : CommandHandler<AddInventoryDocumentLineCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;
    private readonly IInventorySourceBalanceCommandRepository _sourceBalanceRepository;
    private readonly ISerialItemCommandRepository _serialItemCommandRepository;
    private readonly ISerialItemQueryRepository _serialItemQueryRepository;
    private readonly ILogger<AddInventoryDocumentLineCommandHandler> _logger;

    public AddInventoryDocumentLineCommandHandler(
        IInventoryDocumentCommandRepository repository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository,
        ISerialItemCommandRepository serialItemCommandRepository,
        ISerialItemQueryRepository serialItemQueryRepository,
        ILogger<AddInventoryDocumentLineCommandHandler> logger)
    {
        _repository = repository;
        _sourceBalanceRepository = sourceBalanceRepository;
        _serialItemCommandRepository = serialItemCommandRepository;
        _serialItemQueryRepository = serialItemQueryRepository;
        _logger = logger;
    }

    public override async Task<CommandResult<Guid>> Handle(AddInventoryDocumentLineCommand command)
    {
        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
            return Fail("Document not found.");

        if (document.Status != InventoryDocumentStatus.Draft)
            return Fail("Only draft documents can be modified.");

        InventoryDocumentLine? line = null;
        try
        {
            Enum.TryParse<InventoryAdjustmentDirection>(command.Line.AdjustmentDirection, true, out var adjustmentDirection);
            line = InventoryDocumentLine.Create(
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
                command.Line.AdjustmentDirection is null ? null : adjustmentDirection);

            foreach (var serial in command.Line.Serials)
            {
                line.AddSerial(serial.SerialRef, serial.SerialNo);
            }

            document.AddLine(line);

            if (document.DocumentType == InventoryDocumentType.Receipt)
            {
                InventoryDocumentReceiptLotHelper.ApplyReceiptLotBatchNo(document);
            }

            if (InventoryDocumentLineSerialStatusHelper.ShouldReserveSerials(document.DocumentType))
            {
                var serialItems = await InventoryDocumentLineSerialStatusHelper.ResolveSerialItemsAsync(
                    command.Line.Serials.Select(serial => (serial.SerialRef, serial.SerialNo)),
                    command.Line.VariantRef,
                    _serialItemCommandRepository,
                    _serialItemQueryRepository);

                InventoryDocumentLineSerialStatusHelper.ReserveSerialItems(serialItems);
            }

            if (InventoryDocumentLineSourceAllocationHelper.ShouldReserveSourceBalances(document.DocumentType))
            {
                await InventoryDocumentLineSourceAllocationHelper.AllocateSourceBalancesAsync(document, line, _sourceBalanceRepository, _logger);
            }
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(line!.BusinessKey.Value);
    }
}
