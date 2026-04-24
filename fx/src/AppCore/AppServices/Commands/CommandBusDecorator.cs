namespace OysterFx.AppCore.AppServices.Commands; 

using OysterFx.AppCore.Shared.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public abstract class CommandBusDecorator : ICommandBus
{
    #region Fields
    protected ICommandBus _commandBus;
    public abstract int Order { get; }
    #endregion

    #region Constructors
    public CommandBusDecorator()
    {
    }

    #endregion
    public void SetCommandDispatcher(ICommandBus commandDispatcher)
    {
        _commandBus = commandDispatcher;
    }

    #region Abstract Send Commands
    public abstract Task<CommandResult> SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand;

    public abstract Task<CommandResult<TData>> SendAsync<TCommand, TData>(TCommand command) where TCommand : class, ICommand<TData>;
    #endregion
}