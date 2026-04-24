namespace OysterFx.Endpoints.Api.Extensions;

using Microsoft.AspNetCore.Http;
using OysterFx.AppCore.Shared.Commands;
using OysterFx.AppCore.Shared.Events;
using OysterFx.AppCore.Shared.Queries;

public static class HttpContextExtensions
{
    public static ICommandBus CommandBus(this HttpContext httpContext)
        => (ICommandBus)httpContext.RequestServices.GetService(typeof(ICommandBus))!;

    public static IQueryBus QueryBus(this HttpContext httpContext)
        => (IQueryBus)httpContext.RequestServices.GetService(typeof(IQueryBus))!;

    public static IEventBus EventBus(this HttpContext httpContext)
        => (IEventBus)httpContext.RequestServices.GetService(typeof(IEventBus))!;
}