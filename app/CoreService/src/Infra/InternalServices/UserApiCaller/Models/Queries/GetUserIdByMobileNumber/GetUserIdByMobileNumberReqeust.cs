namespace Insurance.Infra.InternalServices.UserApiCaller.Models.Queries.GetUserIdByMobileNumber;

using OysterFx.Infra.ServiceCom.ResutfulApi.Caller;

public class GetUserIdByMobileNumberReqeust : IApiRequest
{
    public string? Path => $"api/Auth/by-mobile/{MobileNumber}";
    public string? MobileNumber { get; set; }
}