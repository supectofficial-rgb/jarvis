namespace Insurance.AppCore.Domain.Profiles.Entities;

/// <summary>
/// اطلاعات محل کار
/// </summary>
public sealed class ProfileWorkplaceInfo
{
    /// <summary>
    /// آدرس
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// کدپستی
    /// </summary>
    public string? PostalCode { get; private set; }

    /// <summary>
    /// شماره تلفن
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// فکس
    /// </summary>
    public string? Fax { get; private set; }

    private ProfileWorkplaceInfo() { }
}