namespace Insurance.UserService.AppCore.AppServices.AAA.Commands.LoginByCredential;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class LoginByCredentialCommandHandler : CommandHandler<LoginByCredentialCommand, LoginByCredentialCommandResult?>
{
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly ILoginCompletionService _loginCompletionService;

    public LoginByCredentialCommandHandler(
        UserManager<Account> userManager,
        SignInManager<Account> signInManager,
        ILoginCompletionService loginCompletionService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _loginCompletionService = loginCompletionService;
    }

    public override async Task<CommandResult<LoginByCredentialCommandResult?>> Handle(LoginByCredentialCommand command)
    {
        if (string.IsNullOrEmpty(command.UserName) || string.IsNullOrEmpty(command.Password))
            return CommandResult<LoginByCredentialCommandResult?>.Failure("نام کاربری یا رمز عبور وارد نشده است");

        var user = await _userManager.FindByNameAsync(command.UserName);
        if (user == null)
            return CommandResult<LoginByCredentialCommandResult?>.Failure("کاربر یافت نشد");

        var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: true);
        if (!passwordCheck.Succeeded)
            return CommandResult<LoginByCredentialCommandResult?>.Failure("نام کاربری یا رمز عبور اشتباه است");

        var (result, error) = await _loginCompletionService.CompleteAsync(user);

        if (!string.IsNullOrWhiteSpace(error) || result is null)
            return CommandResult<LoginByCredentialCommandResult?>.Failure(error ?? "تکمیل فرآیند ورود با خطا مواجه شد");

        return CommandResult<LoginByCredentialCommandResult?>.Success(result);
    }
}
