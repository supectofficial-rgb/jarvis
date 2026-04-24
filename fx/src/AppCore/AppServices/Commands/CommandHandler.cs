namespace OysterFx.AppCore.AppServices.Commands;

using OysterFx.AppCore.Shared.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public abstract class CommandHandler<TCommand, TData> : ICommandHandler<TCommand, TData> where TCommand : ICommand<TData>
{
    protected CommandResult<TData> result = CommandResult<TData>.Default();
    public CommandHandler()
    {
    }

    public abstract Task<CommandResult<TData>> Handle(TCommand command);
    protected virtual Task<CommandResult<TData>> OkAsync(TData data)
    {
        result = CommandResult<TData>.Success(data);
        return Task.FromResult(result);
    }
    protected virtual CommandResult<TData> Ok(TData data)
    {
        result = CommandResult<TData>.Success(data);
        return result;
    }
    protected virtual CommandResult<TData> Fail(string errorMessage)
    {
        result = CommandResult<TData>.Failure(errorMessage);
        return result;
    }

    protected virtual Task<CommandResult<TData>> FailAsync(string errorMessage)
    {
        result = CommandResult<TData>.Failure(errorMessage);
        return Task.FromResult(result);
    }

    protected virtual Task<CommandResult<TData>> ResultAsync(TData data)
    {
        result = CommandResult<TData>.Success(data);
        return Task.FromResult(result);
    }

    protected virtual CommandResult<TData> Result(TData data)
    {
        result = CommandResult<TData>.Success(data);
        return result;
    }
}

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
    protected CommandResult result = CommandResult.Default();
    public CommandHandler() { }
    public abstract Task<CommandResult> Handle(TCommand command);

    protected virtual Task<CommandResult> OkAsync()
    {
        result = CommandResult.Success();
        return Task.FromResult(result);
    }

    protected virtual CommandResult Ok()
    {
        result = CommandResult.Success();
        return result;
    }

    protected virtual CommandResult Fail(string errorMessage)
    {
        result = CommandResult.Failure(errorMessage);
        return result;
    }

    protected virtual Task<CommandResult> FailAsync(string errorMessage)
    {
        result = CommandResult.Failure(errorMessage);
        return Task.FromResult(result);
    }

    protected virtual Task<CommandResult> ResultAsync()
    {
        result = CommandResult.Success();
        return Task.FromResult(result);
    }
    protected virtual CommandResult Result()
    {
        result = CommandResult.Success();
        return result;
    }
}