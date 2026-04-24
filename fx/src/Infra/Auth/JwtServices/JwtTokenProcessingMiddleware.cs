namespace OysterFx.Infra.Auth.JwtServices;

using Microsoft.AspNetCore.Http;
using OysterFx.Infra.Auth.UserServices;

public class JwtTokenProcessingMiddleware
{
    private readonly RequestDelegate _next;

    public JwtTokenProcessingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserInfoService userInfoService)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authzHeader))
        {
            var token = authzHeader.ToString().Replace("Bearer", "");
            if (!string.IsNullOrWhiteSpace(token))
                userInfoService.LoadFromToken(token);
        }


        await _next(context);
    }
}