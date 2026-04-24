namespace OysterFx.Infra.Auth.UserServices;

public interface IUserInfoService
{
    public string UserId { get; }
    public string Username { get; }
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId);

    string GetUserAgent();
    string GetUserIp();
    string GetUserId();
    string GetFirstName();
    string GetLastName();
    string GetUsername();
    string? GetClaim(string claimType);
    bool IsCurrentUser(string userId);
    string UserIdOrDefault();
    string UserIdOrDefault(string defaultValue);
    void LoadFromToken(string token);
}