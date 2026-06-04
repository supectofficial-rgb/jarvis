namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.DeleteInventoryDocumentLine;

using Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;
using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.DeleteInventoryDocumentLine;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class DeleteInventoryDocumentLineCommandHandler : CommandHandler<DeleteInventoryDocumentLineCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;
    private readonly IInventorySourceBalanceCommandRepository _sourceBalanceRepository;
    private readonly ISerialItemCommandRepository _serialItemCommandRepository;
    private readonly ISerialItemQueryRepository _serialItemQueryRepository;

    public DeleteInventoryDocumentLineCommandHandler(
        IInventoryDocumentCommandRepository repository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository,
        ISerialItemCommandRepository serialItemCommandRepository,
        ISerialItemQueryRepository serialItemQueryRepository)
    {
        _repository = repository;
        _sourceBalanceRepository = sourceBalanceRepository;
        _serialItemCommandRepository = serialItemCommandRepository;
        _serialItemQueryRepository = serialItemQueryRepository;
    }

    public override async Task<CommandResult<Guid>> Handle(DeleteInventoryDocumentLineCommand command)
    {
        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
            return Fail("Document not found.");

        if (document.Status != InventoryDocumentStatus.Draft)
            return Fail("Only draft documents can be modified.");

        try
        {
            var line = document.Lines.FirstOrDefault(x => x.BusinessKey.Value == command.LineBusinessKey);
            if (line is null)
                return Fail("Document line was not found.");

            if (InventoryDocumentLineSerialStatusHelper.ShouldReserveSerials(document.DocumentType))
            {
                var previousSerials = await InventoryDocumentLineSerialStatusHelper.ResolveSerialItemsAsync(
                    line.Serials.Select(serial => (serial.SerialRef, serial.SerialNo)),
                    line.VariantRef,
                    _serialItemCommandRepository,
                    _serialItemQueryRepository);
                InventoryDocumentLineSerialStatusHelper.ReleaseSerialItems(previousSerials);
            }

            if (InventoryDocumentLineSourceAllocationHelper.ShouldReserveSourceBalances(document.DocumentType))
            {
                await InventoryDocumentLineSourceAllocationHelper.ReleaseSourceAllocationsAsync(command.LineBusinessKey, _sourceBalanceRepository);
            }

            document.RemoveLine(command.LineBusinessKey);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(command.LineBusinessKey);
    }
}
