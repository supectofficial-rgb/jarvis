namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Commands.CancelInventoryTransaction;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.CancelInventoryTransaction;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CancelInventoryTransactionCommandHandler
    : CommandHandler<CancelInventoryTransactionCommand, CancelInventoryTransactionCommandResult>
{
    private readonly IInventoryTransactionCommandRepository _repository;

    public CancelInventoryTransactionCommandHandler(IInventoryTransactionCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CancelInventoryTransactionCommandResult>> Handle(CancelInventoryTransactionCommand command)
    {
        if (command.TransactionBusinessKey == Guid.Empty)
            return Fail("TransactionBusinessKey is required.");

        var transaction = await _repository.GetByBusinessKeyAsync(command.TransactionBusinessKey);
        if (transaction is null)
            return Fail("Inventory transaction was not found.");

        try
        {
            transaction.CancelDraft(command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new CancelInventoryTransactionCommandResult
        {
            TransactionBusinessKey = transaction.BusinessKey.Value,
            Status = transaction.Status.ToString()
        });
    }
}
