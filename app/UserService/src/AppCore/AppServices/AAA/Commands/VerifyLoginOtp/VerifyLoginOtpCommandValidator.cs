namespace Insurance.UserService.AppCore.AppServices.AAA.Commands.VerifyLoginOtp;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.AAA.Commands.VerifyLoginOtp;

public class VerifyLoginOtpCommandValidator : AbstractValidator<VerifyLoginOtpCommand>
{
    public VerifyLoginOtpCommandValidator()
    {
    }
}