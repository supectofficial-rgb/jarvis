namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.AddInventoryDocumentLine;

using Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;
using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.AddInventoryDocumentLine;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class AddInventoryDocumentLineCommandHandler : CommandHandler<AddInventoryDocumentLineCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;

    public AddInventoryDocumentLineCommandHandler(IInventoryDocumentCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(AddInventoryDocumentLineCommand command)
    {
        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
            return Fail("Document not found.");

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
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(line!.BusinessKey.Value);
    }
}
