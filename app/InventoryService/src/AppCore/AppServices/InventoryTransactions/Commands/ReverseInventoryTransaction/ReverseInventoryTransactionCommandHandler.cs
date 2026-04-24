namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Commands.ReverseInventoryTransaction;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.ReverseInventoryTransaction;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ReverseInventoryTransactionCommandHandler
    : CommandHandler<ReverseInventoryTransactionCommand, ReverseInventoryTransactionCommandResult>
{
    private readonly IInventoryTransactionCommandRepository _repository;

    public ReverseInventoryTransactionCommandHandler(IInventoryTransactionCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ReverseInventoryTransactionCommandResult>> Handle(ReverseInventoryTransactionCommand command)
    {
        if (command.TransactionBusinessKey == Guid.Empty)
            return Fail("TransactionBusinessKey is required.");

        if (command.ReversedByTransactionBusinessKey == Guid.Empty)
            return Fail("ReversedByTransactionBusinessKey is required.");

        var transaction = await _repository.GetByBusinessKeyAsync(command.TransactionBusinessKey);
        if (transaction is null)
            return Fail("Inventory transaction was not found.");

        try
        {
            transaction.MarkReversed(command.ReversedByTransactionBusinessKey, command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new ReverseInventoryTransactionCommandResult
        {
            TransactionBusinessKey = transaction.BusinessKey.Value,
            ReversedByTransactionBusinessKey = command.ReversedByTransactionBusinessKey,
            Status = transaction.Status.ToString()
        });
    }
}
