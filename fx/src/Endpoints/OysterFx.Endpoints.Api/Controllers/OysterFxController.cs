namespace OysterFx.Endpoints.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OysterFx.AppCore.Shared.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using OysterFx.AppCore.Shared.Events;
using OysterFx.AppCore.Shared.Queries;
using OysterFx.AppCore.Shared.Queries.Common;
using OysterFx.Endpoints.Api.Extensions;
using System.Net;

public class OysterFxController : Controller
{
    protected ICommandBus CommandBus => HttpContext.CommandBus();
    protected IQueryBus QueryBus => HttpContext.QueryBus();
    protected IEventBus EventBus => HttpContext.EventBus();

    protected async Task<IActionResult> SendCommand<TCommand, TCommandResult>(TCommand command) where TCommand : class, ICommand<TCommandResult>
    {
        var result = await CommandBus.SendAsync<TCommand, TCommandResult>(command);
        if (result.IsSuccess)
            return StatusCode((int)HttpStatusCode.Created, result);

        return BadRequest(result);
    }

    protected async Task<CommandResult<TCommandResult>> CommandAsync<TCommand, TCommandResult>(TCommand command) where TCommand : class, ICommand<TCommandResult>
    {
        var result = await CommandBus.SendAsync<TCommand, TCommandResult>(command);
        return result;
    }

    protected async Task<IActionResult> SendCommand<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        var result = await CommandBus.SendAsync(command);
        if (result.IsSuccess)
        {
            return StatusCode((int)HttpStatusCode.Created);
        }
        return BadRequest(result);
    }

    protected async Task<IActionResult> ExecuteQueryAsync<TQuery, TQueryResult>(TQuery query) where TQuery : class, IQuery<TQueryResult>
    {
        var result = await QueryBus.ExecuteAsync<TQuery, TQueryResult>(query);

        if (!result.IsSuccess)
            return BadRequest(result);
        else
            return Ok(result);
    }
}