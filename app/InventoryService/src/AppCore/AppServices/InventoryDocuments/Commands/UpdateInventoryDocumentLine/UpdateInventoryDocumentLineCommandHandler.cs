namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.UpdateInventoryDocumentLine;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.UpdateInventoryDocumentLine;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class UpdateInventoryDocumentLineCommandHandler : CommandHandler<UpdateInventoryDocumentLineCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;

    public UpdateInventoryDocumentLineCommandHandler(IInventoryDocumentCommandRepository repository)
    {
        _repository = repository;
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
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(command.LineBusinessKey);
    }
}
