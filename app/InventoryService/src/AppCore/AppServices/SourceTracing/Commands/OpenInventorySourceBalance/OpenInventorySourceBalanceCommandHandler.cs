namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Commands.OpenInventorySourceBalance;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.OpenInventorySourceBalance;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class OpenInventorySourceBalanceCommandHandler : CommandHandler<OpenInventorySourceBalanceCommand, Guid>
{
    private readonly IInventorySourceBalanceCommandRepository _repository;

    public OpenInventorySourceBalanceCommandHandler(IInventorySourceBalanceCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(OpenInventorySourceBalanceCommand command)
    {
        if (!Enum.TryParse<InventorySourceType>(command.SourceType, true, out var sourceType))
            return Fail($"Unsupported source type '{command.SourceType}'.");

        InventorySourceBalance aggregate;
        try
        {
            aggregate = InventorySourceBalance.Open(
                sourceType,
                command.VariantRef,
                command.SellerRef,
                command.WarehouseRef,
                command.LocationRef,
                command.QualityStatusRef,
                command.BaseUomRef,
                command.ReceivedQty,
                command.LotBatchNo,
                command.SourceDocumentRef,
                command.SourceDocumentLineRef,
                command.SourceTransactionRef,
                command.SourceTransactionLineRef,
                command.SerialRef);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.InsertAsync(aggregate);
        await _repository.CommitAsync();
        return Ok(aggregate.BusinessKey.Value);
    }
}
