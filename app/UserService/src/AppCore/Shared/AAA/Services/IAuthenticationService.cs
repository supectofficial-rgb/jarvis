namespace Insurance.UserService.AppCore.Shared.AAA.Services;

using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Users.Dtos;
using OysterFx.AppCore.Domain.ValueObjects;

public interface IAuthenticationService
{
    Task<AuthResult> LoginAsync(string username, string password);
    Task<AuthResult> LoginWithPersonaAsync(string username, string password, BusinessKey personaKey);
    Task LogoutAsync();
    Task<UserContext> GetCurrentUserContextAsync();
}