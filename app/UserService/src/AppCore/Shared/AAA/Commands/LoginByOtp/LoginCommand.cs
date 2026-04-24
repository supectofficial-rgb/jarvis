namespace Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByOtp;

using OysterFx.AppCore.Shared.Commands;

public class LoginByOtpCommand : ICommand<LoginByOtpCommandResult?>
{
    public string? MobileNumber { get; set; }

    public LoginByOtpCommand(string? mobileNumber)
    {
        MobileNumber = mobileNumber;
    }

    public static LoginByOtpCommand CreateInstance(string? mobileNumber)
        => new LoginByOtpCommand(mobileNumber);
}