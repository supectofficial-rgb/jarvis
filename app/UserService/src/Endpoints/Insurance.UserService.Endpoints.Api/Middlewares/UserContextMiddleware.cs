namespace Insurance.UserService.Endpoints.Api.Middlewares;

using Insurance.UserService.AppCore.Shared.AAA.Services;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Security.Claims;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserContextService userContextService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("userId")?.Value
                ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var personaKey = context.User.FindFirst("activePersonaBusinessKey")?.Value
                ?? context.User.FindFirst("activeMembershipBusinessKey")?.Value
                ?? context.User.FindFirst("currentMembershipKey")?.Value
                ?? context.User.FindFirst("PersonaKey")?.Value;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(personaKey))
            {
                var userContext = await userContextService.BuildUserContextAsync(
                    long.Parse(userId),
                    BusinessKey.FromGuid(Guid.Parse(personaKey)));

                context.Items["UserContext"] = userContext;
            }
        }

        await _next(context);
    }
}
