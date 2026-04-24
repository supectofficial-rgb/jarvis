namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Commands.ActivateSeller;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.ActivateSeller;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateSellerCommandHandler : CommandHandler<ActivateSellerCommand, ActivateSellerCommandResult>
{
    private readonly ISellerCommandRepository _repository;

    public ActivateSellerCommandHandler(ISellerCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ActivateSellerCommandResult>> Handle(ActivateSellerCommand command)
    {
        if (command.SellerBusinessKey == Guid.Empty)
            return Fail("SellerBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.SellerBusinessKey);
        if (aggregate is null)
            return Fail("Seller was not found.");

        aggregate.Activate();
        await _repository.CommitAsync();

        return Ok(new ActivateSellerCommandResult
        {
            SellerBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}
