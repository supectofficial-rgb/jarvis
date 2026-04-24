namespace Insurance.Infra.InternalServices.UserApiCaller.ServiceCallers;

using Insurance.Infra.InternalServices.UserApiCaller.Abstractions;
using Insurance.Infra.InternalServices.UserApiCaller.Models.Commands;
using Insurance.Infra.InternalServices.UserApiCaller.Models.Constants;
using Insurance.Infra.InternalServices.UserApiCaller.Models.Queries.GetUserIdByMobileNumber;
using Microsoft.Extensions.Options;
using OysterFx.Infra.Auth.JwtServices;

public class UserService : IUserService
{
    private readonly IOptions<UserServiceOptions> _userOptions;
    private readonly HttpService _httpService;
    private readonly IGenerateDafaultJwtTokenService _generateDafaultJwtTokenService;
    private readonly string _baseURL = string.Empty;
    public UserService(
        IOptions<UserServiceOptions> userOptions,
        HttpService httpService,
        IGenerateDafaultJwtTokenService generateDafaultJwtTokenService)
    {
        _userOptions = userOptions;
        _httpService = httpService;
        _generateDafaultJwtTokenService = generateDafaultJwtTokenService;
        _baseURL = _userOptions.Value.BaseURL;
    }

    public async Task<GetUserIdByMobileNumberResponse> GetUserIdByMobileNumber(GetUserIdByMobileNumberReqeust request, string baseUrl = "")
    {
        var (success, error) = await _httpService.GetAsync<GetUserIdByMobileNumberResponse, List<string>>($"{_baseURL}{request.Path}");
        return success!;
    }

    public async Task<(RegisterUserResponse Success, List<string> Errors)> Register(RegisterUserRequest request, string baseUrl = "")
    {
        var (success, error) = await _httpService.PostAsync<RegisterUserRequest, RegisterUserResponse, List<string>>($"{_baseURL}api/Auth/register", request);

        return (success, error)!;
    }

    private string GetJwtToken() => _generateDafaultJwtTokenService.ExecuteAsync().Result;
}