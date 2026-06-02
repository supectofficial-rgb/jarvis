namespace Insurance.UserService.AppCore.Shared.AAA.Services;

using Insurance.UserService.AppCore.Domain.Common;
using OysterFx.AppCore.Domain.ValueObjects;

public interface IAuthSessionService
{
    Task CreateAsync(AuthSession session, int absoluteExpirationMinutes, CancellationToken cancellationToken = default);
    Task<AuthSession?> GetAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<bool> RevokeAsync(string sessionId, string reason, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(AuthSession session, int absoluteExpirationMinutes, CancellationToken cancellationToken = default);
    string HashRefreshToken(string refreshToken);
    bool VerifyRefreshToken(string refreshToken, string expectedHash);
    string BuildKey(string sessionId);
}
