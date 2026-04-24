namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Commands.EnsureStockDetailBucket;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.EnsureStockDetailBucket;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class EnsureStockDetailBucketCommandHandler : CommandHandler<EnsureStockDetailBucketCommand, EnsureStockDetailBucketCommandResult>
{
    private readonly IStockDetailCommandRepository _repository;

    public EnsureStockDetailBucketCommandHandler(IStockDetailCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<EnsureStockDetailBucketCommandResult>> Handle(EnsureStockDetailBucketCommand command)
    {
        if (command.VariantRef == Guid.Empty || command.SellerRef == Guid.Empty || command.WarehouseRef == Guid.Empty ||
            command.LocationRef == Guid.Empty || command.QualityStatusRef == Guid.Empty)
        {
            return Fail("Bucket dimensions are required.");
        }

        if (command.OpeningQuantityOnCreate < 0)
            return Fail("OpeningQuantityOnCreate cannot be negative.");

        var existing = await _repository.FindByBucketAsync(
            command.VariantRef,
            command.SellerRef,
            command.WarehouseRef,
            command.LocationRef,
            command.QualityStatusRef,
            string.IsNullOrWhiteSpace(command.LotBatchNo) ? null : command.LotBatchNo.Trim());

        if (existing is not null)
        {
            return Ok(new EnsureStockDetailBucketCommandResult
            {
                StockDetailBusinessKey = existing.BusinessKey.Value,
                Created = false,
                QuantityOnHand = existing.QuantityOnHand
            });
        }

        var aggregate = StockDetail.Create(
            command.VariantRef,
            command.SellerRef,
            command.WarehouseRef,
            command.LocationRef,
            command.QualityStatusRef,
            command.LotBatchNo,
            command.OpeningQuantityOnCreate,
            command.OccurredAt);

        await _repository.InsertAsync(aggregate);
        await _repository.CommitAsync();

        return Ok(new EnsureStockDetailBucketCommandResult
        {
            StockDetailBusinessKey = aggregate.BusinessKey.Value,
            Created = true,
            QuantityOnHand = aggregate.QuantityOnHand
        });
    }
}
