namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Commands.UnsetSellerAsSystemOwner;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.UnsetSellerAsSystemOwner;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UnsetSellerAsSystemOwnerCommandHandler : CommandHandler<UnsetSellerAsSystemOwnerCommand, UnsetSellerAsSystemOwnerCommandResult>
{
    private readonly ISellerCommandRepository _repository;

    public UnsetSellerAsSystemOwnerCommandHandler(ISellerCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UnsetSellerAsSystemOwnerCommandResult>> Handle(UnsetSellerAsSystemOwnerCommand command)
    {
        if (command.SellerBusinessKey == Guid.Empty)
            return Fail("SellerBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.SellerBusinessKey);
        if (aggregate is null)
            return Fail("Seller was not found.");

        aggregate.SetSystemOwner(false);
        await _repository.CommitAsync();

        return Ok(new UnsetSellerAsSystemOwnerCommandResult
        {
            SellerBusinessKey = aggregate.BusinessKey.Value,
            IsSystemOwner = aggregate.IsSystemOwner
        });
    }
}
