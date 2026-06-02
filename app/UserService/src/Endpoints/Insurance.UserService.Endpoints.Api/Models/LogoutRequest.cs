namespace Insurance.UserService.Endpoints.Api.Models;

public sealed class LogoutRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
