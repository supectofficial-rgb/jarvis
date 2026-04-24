namespace Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByOtp;

using System;

public class LoginByOtpCommandResult
{
    public bool OtpSended { get; set; }
    public TimeSpan ExpirationTime { get; set; }
}
