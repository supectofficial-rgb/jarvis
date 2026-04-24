namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Commands.SetSellerAsSystemOwner;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.SetSellerAsSystemOwner;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class SetSellerAsSystemOwnerCommandHandler : CommandHandler<SetSellerAsSystemOwnerCommand, SetSellerAsSystemOwnerCommandResult>
{
    private readonly ISellerCommandRepository _repository;

    public SetSellerAsSystemOwnerCommandHandler(ISellerCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<SetSellerAsSystemOwnerCommandResult>> Handle(SetSellerAsSystemOwnerCommand command)
    {
        if (command.SellerBusinessKey == Guid.Empty)
            return Fail("SellerBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.SellerBusinessKey);
        if (aggregate is null)
            return Fail("Seller was not found.");

        aggregate.SetSystemOwner(command.IsSystemOwner);
        await _repository.CommitAsync();

        return Ok(new SetSellerAsSystemOwnerCommandResult
        {
            SellerBusinessKey = aggregate.BusinessKey.Value,
            IsSystemOwner = aggregate.IsSystemOwner
        });
    }
}
