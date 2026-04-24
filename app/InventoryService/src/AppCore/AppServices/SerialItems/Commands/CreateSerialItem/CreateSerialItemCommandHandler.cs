namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Commands.CreateSerialItem;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.CreateSerialItem;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateSerialItemCommandHandler : CommandHandler<CreateSerialItemCommand, CreateSerialItemCommandResult>
{
    private readonly ISerialItemCommandRepository _repository;

    public CreateSerialItemCommandHandler(ISerialItemCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateSerialItemCommandResult>> Handle(CreateSerialItemCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.SerialNo))
            return Fail("SerialNo is required.");

        if (command.VariantRef == Guid.Empty || command.SellerRef == Guid.Empty || command.WarehouseRef == Guid.Empty ||
            command.LocationRef == Guid.Empty || command.QualityStatusRef == Guid.Empty)
        {
            return Fail("Serial dimensions are required.");
        }

        if (await _repository.ExistsBySerialNoAsync(command.VariantRef, command.SerialNo))
            return Fail("SerialNo already exists for this variant.");

        var aggregate = SerialItem.Create(
            command.SerialNo,
            command.VariantRef,
            command.SellerRef,
            command.WarehouseRef,
            command.LocationRef,
            command.QualityStatusRef,
            command.LotBatchNo);

        await _repository.InsertAsync(aggregate);
        await _repository.CommitAsync();

        return Ok(new CreateSerialItemCommandResult
        {
            SerialItemBusinessKey = aggregate.BusinessKey.Value,
            SerialNo = aggregate.SerialNo,
            Status = aggregate.Status.ToString()
        });
    }
}
