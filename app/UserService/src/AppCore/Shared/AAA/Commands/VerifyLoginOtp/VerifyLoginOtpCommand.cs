namespace Insurance.UserService.AppCore.Shared.AAA.Commands.VerifyLoginOtp;

using OysterFx.AppCore.Shared.Commands;

public class VerifyLoginOtpCommand : ICommand<VerifyLoginOtpCommandResult?>
{
    public string? MobileNumber { get; set; }
    public string? Code { get; set; }

    public VerifyLoginOtpCommand(string? mobileNumber, string? code)
    {
        MobileNumber = mobileNumber;
        Code = code;
    }

    public static VerifyLoginOtpCommand CreateInstance(string? mobileNumber, string? code)
        => new(mobileNumber, code);
}