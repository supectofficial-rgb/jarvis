namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Commands.DeleteSeller;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.DeleteSeller;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteSellerCommandHandler : CommandHandler<DeleteSellerCommand, DeleteSellerCommandResult>
{
    private readonly ISellerCommandRepository _repository;

    public DeleteSellerCommandHandler(ISellerCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<DeleteSellerCommandResult>> Handle(DeleteSellerCommand command)
    {
        if (command.SellerBusinessKey == Guid.Empty)
            return Fail("SellerBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.SellerBusinessKey);
        if (aggregate is null)
            return Fail("Seller was not found.");

        aggregate.Deactivate();
        aggregate.SetSystemOwner(false);
        await _repository.CommitAsync();

        return Ok(new DeleteSellerCommandResult
        {
            SellerBusinessKey = aggregate.BusinessKey.Value,
            Deleted = true
        });
    }
}
