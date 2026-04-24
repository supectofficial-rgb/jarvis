namespace Insurance.CoreService.Endpoints.Api.Middlewares;

using OysterFx.AppCore.Domain.BusinessRules;
using OysterFx.AppCore.Shared.Commands.Common;
using System.Net;
using System.Text.Json;

public class BusinessRuleExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public BusinessRuleExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            CommandResult<object> result;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            if (ex.InnerException is BusinessRuleValidationException brve)
            {
                result = CommandResult<object>.Failure($"{brve.Message}");
                await context.Response.WriteAsync(JsonSerializer.Serialize(result, options));
                return;
            }

            result = CommandResult<object>.Failure($"{ex.Message}");
            await context.Response.WriteAsync(JsonSerializer.Serialize(result, options));
        }
    }
}