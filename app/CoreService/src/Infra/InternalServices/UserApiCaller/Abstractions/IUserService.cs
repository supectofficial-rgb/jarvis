namespace Insurance.Infra.InternalServices.UserApiCaller.Abstractions;

using Insurance.Infra.InternalServices.UserApiCaller.Models.Commands;
using Insurance.Infra.InternalServices.UserApiCaller.Models.Queries.GetUserIdByMobileNumber;

public interface IUserService
{
    public Task<(RegisterUserResponse Success, List<string> Errors)> Register(RegisterUserRequest request, string baseUrl = "");
    public Task<GetUserIdByMobileNumberResponse> GetUserIdByMobileNumber(GetUserIdByMobileNumberReqeust request, string baseUrl = "");
}