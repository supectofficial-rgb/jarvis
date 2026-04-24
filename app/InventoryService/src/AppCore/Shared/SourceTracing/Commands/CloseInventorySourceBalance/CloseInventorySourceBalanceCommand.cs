namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.CloseInventorySourceBalance;

using OysterFx.AppCore.Shared.Commands;

public class CloseInventorySourceBalanceCommand : ICommand<Guid>
{
    public Guid SourceBalanceBusinessKey { get; set; }
}
