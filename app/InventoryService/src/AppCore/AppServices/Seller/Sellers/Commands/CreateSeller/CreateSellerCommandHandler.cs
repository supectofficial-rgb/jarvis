namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Commands.CreateSeller;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.CreateSeller;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateSellerCommandHandler : CommandHandler<CreateSellerCommand, CreateSellerCommandResult>
{
    private readonly ISellerCommandRepository _repository;

    public CreateSellerCommandHandler(ISellerCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateSellerCommandResult>> Handle(CreateSellerCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var normalizedCode = command.Code.Trim();
        if (await _repository.ExistsByCodeAsync(normalizedCode))
            return Fail($"Seller code '{normalizedCode}' already exists.");

        var aggregate = Domain.Seller.Entities.Seller.Create(normalizedCode, command.Name.Trim(), command.IsSystemOwner);

        await _repository.InsertAsync(aggregate);
        await _repository.CommitAsync();

        return Ok(new CreateSellerCommandResult
        {
            SellerBusinessKey = aggregate.BusinessKey.Value,
            Code = aggregate.Code,
            Name = aggregate.Name,
            IsSystemOwner = aggregate.IsSystemOwner,
            IsActive = aggregate.IsActive
        });
    }
}
