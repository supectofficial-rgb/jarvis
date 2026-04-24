namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.PostInventoryTransaction;

using OysterFx.AppCore.Shared.Commands;

public class PostInventoryTransactionCommand : ICommand<PostInventoryTransactionCommandResult>
{
    public Guid TransactionBusinessKey { get; set; }
}
