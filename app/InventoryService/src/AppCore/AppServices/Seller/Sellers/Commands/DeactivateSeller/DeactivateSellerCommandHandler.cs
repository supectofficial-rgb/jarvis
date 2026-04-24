namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Commands.DeactivateSeller;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.DeactivateSeller;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateSellerCommandHandler : CommandHandler<DeactivateSellerCommand, DeactivateSellerCommandResult>
{
    private readonly ISellerCommandRepository _repository;

    public DeactivateSellerCommandHandler(ISellerCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<DeactivateSellerCommandResult>> Handle(DeactivateSellerCommand command)
    {
        if (command.SellerBusinessKey == Guid.Empty)
            return Fail("SellerBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.SellerBusinessKey);
        if (aggregate is null)
            return Fail("Seller was not found.");

        aggregate.Deactivate();
        await _repository.CommitAsync();

        return Ok(new DeactivateSellerCommandResult
        {
            SellerBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}
