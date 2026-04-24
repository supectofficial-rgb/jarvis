namespace Insurance.UserService.AppCore.Domain.Common;

public class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public long UserId { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
}