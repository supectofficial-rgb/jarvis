namespace Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;

using OysterFx.AppCore.Shared.Commands;

public class LoginByCredentialCommand : ICommand<LoginByCredentialCommandResult?>
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}