namespace Insurance.UserService.AppCore.AppServices.AAA.Commands.LoginByOtp;

using Insurance.PanelPayamakService.Infra.Abstractions;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByOtp;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class LoginByOtpCommandHandler : CommandHandler<LoginByOtpCommand, LoginByOtpCommandResult?>
{
    private readonly IPayamakSender _payamakSender;
    private readonly IUserOtpService _userOtpService;

    public LoginByOtpCommandHandler(
        IPayamakSender payamakSender,
        IUserOtpService userOtpService)
    {
        _payamakSender = payamakSender;
        _userOtpService = userOtpService;
    }

    public override async Task<CommandResult<LoginByOtpCommandResult?>> Handle(LoginByOtpCommand command)
    {
        var otp = await _userOtpService.CreateAsync(command.MobileNumber!);

        if (!otp.Success || string.IsNullOrWhiteSpace(otp.Code))
        {
            return CommandResult<LoginByOtpCommandResult?>.Failure(
                otp.Error ?? "??? ?? ????? ??",
                new()
                {
                    OtpSended = false
                });
        }

        var sendResult = await _payamakSender.ExecuteAsync(new()
        {
            Message = otp.Code,
            Receptor = command.MobileNumber
        });

        if (sendResult.IsSuccess)
        {
            return CommandResult<LoginByOtpCommandResult?>.Success(new()
            {
                OtpSended = true,
                ExpirationTime = otp.Expiration ?? TimeSpan.FromMinutes(2)
            });
        }

        return CommandResult<LoginByOtpCommandResult?>.Failure("??? ?? ????? ??", new()
        {
            OtpSended = false
        });
    }
}
