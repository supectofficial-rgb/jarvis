namespace Insurance.UserService.AppCore.Shared.AAA.Services;

using Insurance.UserService.AppCore.Domain.Users.Dtos;
using OysterFx.AppCore.Domain.ValueObjects;

public interface IUserContextService
{
    Task<UserContext> BuildUserContextAsync(long userId, BusinessKey? personaKey = null);
    Task<UserContext?> GetCurrentUserContextAsync();
    Task<bool> SwitchPersonaAsync(BusinessKey newPersonaKey);
    Task UpdateLastActivityAsync();
    Task ClearUserContextAsync();
}
