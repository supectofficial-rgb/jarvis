namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Commands.PostInventoryTransaction;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.PostInventoryTransaction;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class PostInventoryTransactionCommandHandler
    : CommandHandler<PostInventoryTransactionCommand, PostInventoryTransactionCommandResult>
{
    private readonly IInventoryTransactionCommandRepository _repository;

    public PostInventoryTransactionCommandHandler(IInventoryTransactionCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<PostInventoryTransactionCommandResult>> Handle(PostInventoryTransactionCommand command)
    {
        if (command.TransactionBusinessKey == Guid.Empty)
            return Fail("TransactionBusinessKey is required.");

        var transaction = await _repository.GetByBusinessKeyAsync(command.TransactionBusinessKey);
        if (transaction is null)
            return Fail("Inventory transaction was not found.");

        try
        {
            transaction.MarkPosted();
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new PostInventoryTransactionCommandResult
        {
            TransactionBusinessKey = transaction.BusinessKey.Value,
            Status = transaction.Status.ToString()
        });
    }
}
