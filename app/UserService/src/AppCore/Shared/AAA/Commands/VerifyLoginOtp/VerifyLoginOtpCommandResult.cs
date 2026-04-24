namespace Insurance.UserService.AppCore.Shared.AAA.Commands.VerifyLoginOtp;

using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;

public class VerifyLoginOtpCommandResult
{
    public bool Verified { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }

    public LoginByCredentialCommandResult? LoginResult { get; set; }
}
