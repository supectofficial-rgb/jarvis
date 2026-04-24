namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Commands.UpdateSeller;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.UpdateSeller;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateSellerCommandHandler : CommandHandler<UpdateSellerCommand, UpdateSellerCommandResult>
{
    private readonly ISellerCommandRepository _repository;

    public UpdateSellerCommandHandler(ISellerCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdateSellerCommandResult>> Handle(UpdateSellerCommand command)
    {
        if (command.SellerBusinessKey == Guid.Empty)
            return Fail("SellerBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.SellerBusinessKey);
        if (aggregate is null)
            return Fail("Seller was not found.");

        var normalizedCode = command.Code.Trim();
        if (!string.Equals(aggregate.Code, normalizedCode, StringComparison.OrdinalIgnoreCase)
            && await _repository.ExistsByCodeAsync(normalizedCode, command.SellerBusinessKey))
        {
            return Fail($"Seller code '{normalizedCode}' already exists.");
        }

        aggregate.ChangeCode(normalizedCode);
        aggregate.Rename(command.Name.Trim());
        aggregate.SetSystemOwner(command.IsSystemOwner);

        if (command.IsActive)
            aggregate.Activate();
        else
            aggregate.Deactivate();

        await _repository.CommitAsync();

        return Ok(new UpdateSellerCommandResult
        {
            SellerBusinessKey = aggregate.BusinessKey.Value,
            Code = aggregate.Code,
            Name = aggregate.Name,
            IsSystemOwner = aggregate.IsSystemOwner,
            IsActive = aggregate.IsActive
        });
    }
}
