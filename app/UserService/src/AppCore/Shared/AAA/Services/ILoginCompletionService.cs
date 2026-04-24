namespace Insurance.UserService.AppCore.Shared.AAA.Services;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;

public interface ILoginCompletionService
{
    Task<(LoginByCredentialCommandResult? Result, string? Error)> CompleteAsync(Account user);
}
