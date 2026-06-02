namespace Insurance.UserService.AppCore.Domain.Common;

using OysterFx.AppCore.Domain.ValueObjects;

public sealed class AuthSession
{
    public string SessionId { get; set; } = string.Empty;
    public long UserId { get; set; }
    public BusinessKey UserBusinessKey { get; set; }
    public string RefreshTokenHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
