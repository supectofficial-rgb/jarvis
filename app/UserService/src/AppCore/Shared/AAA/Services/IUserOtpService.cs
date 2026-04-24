namespace Insurance.UserService.AppCore.Shared.AAA.Services;

public interface IUserOtpService
{
    Task<(bool Success, string? Code, string? Error, TimeSpan? Expiration)> CreateAsync(string mobileNumber);
    Task<(bool Verified, string Message)> VerifyAsync(string mobileNumber, string code);
}
