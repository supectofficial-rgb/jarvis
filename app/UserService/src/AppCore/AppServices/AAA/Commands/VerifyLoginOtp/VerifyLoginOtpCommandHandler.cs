namespace Insurance.UserService.AppCore.AppServices.AAA.Commands.VerifyLoginOtp;

using Insurance.Infra.InternalServices.UserApiCaller.Abstractions;
using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Commands.VerifyLoginOtp;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class VerifyLoginOtpCommandHandler : CommandHandler<VerifyLoginOtpCommand, VerifyLoginOtpCommandResult?>
{
    private readonly IUserOtpService _userOtpService;
    private readonly IUserService _userService;
    private readonly UserManager<Account> _userManager;
    private readonly ILoginCompletionService _loginCompletionService;

    public VerifyLoginOtpCommandHandler(
        IUserOtpService userOtpService,
        IUserService userService,
        UserManager<Account> userManager,
        ILoginCompletionService loginCompletionService)
    {
        _userOtpService = userOtpService;
        _userService = userService;
        _userManager = userManager;
        _loginCompletionService = loginCompletionService;
    }

    public override async Task<CommandResult<VerifyLoginOtpCommandResult?>> Handle(VerifyLoginOtpCommand command)
    {
        var verifyCommandResult = new VerifyLoginOtpCommandResult();

        var otpVerify = await _userOtpService.VerifyAsync(command.MobileNumber!, command.Code!);
        if (!otpVerify.Verified)
        {
            verifyCommandResult.Verified = false;
            verifyCommandResult.Message = otpVerify.Message;
            return await OkAsync(verifyCommandResult);
        }

        var userResponse = await _userService.GetUserIdByMobileNumber(new()
        {
            MobileNumber = command.MobileNumber
        });

        var resolvedUserId = userResponse?.UserId;

        if (string.IsNullOrWhiteSpace(resolvedUserId))
        {
            var registeredUser = await _userService.Register(new()
            {
                MobileNumber = command.MobileNumber
            });

            resolvedUserId = registeredUser.Success?.UserId;
        }

        if (string.IsNullOrWhiteSpace(resolvedUserId))
        {
            verifyCommandResult.Verified = false;
            verifyCommandResult.Message = "????? ???? ???? ???? ???";
            return await OkAsync(verifyCommandResult);
        }

        var account = await _userManager.FindByIdAsync(resolvedUserId)
            ?? await _userManager.FindByNameAsync(command.MobileNumber!);

        if (account is null)
        {
            verifyCommandResult.Verified = false;
            verifyCommandResult.Message = "???? ?????? ???? ???? ???? ???";
            return await OkAsync(verifyCommandResult);
        }

        var (loginResult, error) = await _loginCompletionService.CompleteAsync(account);

        if (!string.IsNullOrWhiteSpace(error) || loginResult is null)
        {
            verifyCommandResult.Verified = false;
            verifyCommandResult.Message = error ?? "????? ?????? ???? ?? ??? ????? ??";
            return await OkAsync(verifyCommandResult);
        }

        verifyCommandResult.Token = loginResult.Token;
        verifyCommandResult.LoginResult = loginResult;
        verifyCommandResult.Verified = true;
        verifyCommandResult.Message = otpVerify.Message;

        return await OkAsync(verifyCommandResult);
    }
}
