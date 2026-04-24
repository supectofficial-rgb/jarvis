namespace OysterFx.AppCore.Shared.Commands;

using OysterFx.AppCore.Shared.Commands.Common;

public interface ICommandBus
{
    Task<CommandResult> SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand;
    Task<CommandResult<TData>> SendAsync<TCommand, TData>(TCommand command) where TCommand : class, ICommand<TData>;
}