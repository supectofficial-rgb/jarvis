namespace Insurance.UserService.AppCore.Domain.Users.Entities;

using OysterFx.AppCore.Domain.Aggregates;

/// <summary>
/// کاربر -Aggregate Root
/// </summary>
public sealed class User : AggregateRoot
{
    public string? MobileNumber { get; private set; }
    public string? Code { get; private set; }
    public string? FullName { get; private set; }
    public string? City { get; private set; }
    public string? Province { get; private set; }

    public decimal MaxAllowedExpertiseAmount { get; private set; }
    public decimal MaxAllowedDailyCaseReferral { get; private set; }
    public decimal MaxAllowedOpenCases { get; private set; }

    public string? ProfileImageBase64 { get; private set; }

    public DateTime? ExpirationDate { get; private set; }

    private User() { }

    private User(string mobileNumber)
    {
        MobileNumber = mobileNumber;
    }

    public static User NewUser(string mobileNumber) => new(mobileNumber);
}