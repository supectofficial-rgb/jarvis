namespace Insurance.Infra.InternalServices.UserApiCaller.Models.Commands;

using OysterFx.Infra.ServiceCom.ResutfulApi.Caller;

public class RegisterUserRequest : IApiRequest
{
    public string? Path => "api/Auth/register";
    public string? MobileNumber { get; set; }
}