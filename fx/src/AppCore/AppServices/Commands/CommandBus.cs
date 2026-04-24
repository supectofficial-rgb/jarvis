namespace OysterFx.AppCore.AppServices.Commands; 

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.Shared.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CommandBus : ICommandBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandBus> _logger;

    public CommandBus(IServiceProvider serviceProvider, ILogger<CommandBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<CommandResult> SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        try
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            return await handler.Handle(command);
        }
        catch (Exception ex)
        {
            _logger.LogError("CommandBus Dispatch exception occured CommandType: {@commandType} Command: {@command} ex: {@ex}", command.GetType(), command, ex);
            throw;
        }
    }

    public async Task<CommandResult<TData>> SendAsync<TCommand, TData>(TCommand command) where TCommand : class, ICommand<TData>
    {
        try
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TData>>();
            return await handler.Handle(command);
        }
        catch (Exception ex)
        {
            _logger.LogError("CommandBus Dispatch exception occured CommandType: {@commandType} Command: {@command} ex: {@ex}", command.GetType(), command, ex);
            throw;
        }
    }
}

public class ValidatingCommandBusDecorator : ICommandBus
{
    private readonly ICommandBus _inner;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidatingCommandBusDecorator> _logger;

    public ValidatingCommandBusDecorator(
        ICommandBus inner,
        IServiceProvider serviceProvider,
        ILogger<ValidatingCommandBusDecorator> logger)
    {
        _inner = inner;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    public async Task<CommandResult> SendAsync<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        try
        {
            // Find validator via reflection
            var validatorType = typeof(IValidator<>).MakeGenericType(command.GetType());
            if (_serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var validationResult = await validator.ValidateAsync(
                    new ValidationContext<TCommand>(command));

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage).GetMessage())
                        .ToList();
                    return CommandResult.Failure(errors);
                }
            }

            return await _inner.SendAsync(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validation failed for command {CommandType}", command.GetType());
            throw;
        }
    }

    public async Task<CommandResult<TData>> SendAsync<TCommand, TData>(TCommand command)
        where TCommand : class, ICommand<TData>
    {
        try
        {
            // Same validation logic as above
            var validatorType = typeof(IValidator<>).MakeGenericType(command.GetType());
            if (_serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var validationResult = await validator.ValidateAsync(
                    new ValidationContext<TCommand>(command));

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage).GetMessage())
                        .ToList();
                    return CommandResult<TData>.Failure(errors);
                }
            }

            return await _inner.SendAsync<TCommand, TData>(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validation failed for command {CommandType}", command.GetType());
            throw;
        }
    }
}