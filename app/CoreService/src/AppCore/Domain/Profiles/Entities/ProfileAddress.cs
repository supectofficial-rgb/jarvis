namespace Insurance.AppCore.Domain.Profiles.Entities;

/// <summary>
/// اطلاعات محل سکونت
/// </summary>
public sealed class ProfileAddress
{
    /// <summary>
    /// استان
    /// </summary>
    public string? Province { get; private set; }

    /// <summary>
    /// شهر
    /// </summary>
    public string? City { get; private set; }

    /// <summary>
    /// آدرس
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// کد پستی
    /// </summary>
    public string? PostalCode { get; private set; }

    /// <summary>
    /// شماره تلفن
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// شماره موبایل
    /// </summary>
    public string? MobileNumber { get; private set; }

    private ProfileAddress() { }

    public static ProfileAddress Create(
        string? province,
        string? city,
        string? address,
        string? postalCode,
        string? phoneNumber,
        string? mobileNumber)
    {
        return new ProfileAddress
        {
            Province = province,
            City = city,
            Address = address,
            PostalCode = postalCode,
            PhoneNumber = phoneNumber,
            MobileNumber = mobileNumber
        };
    }

    public void UpdateAddress(
        string? province = null,
        string? city = null,
        string? address = null,
        string? postalCode = null,
        string? phoneNumber = null,
        string? mobileNumber = null)
    {
        if (!string.IsNullOrWhiteSpace(province)) Province = province;
        if (!string.IsNullOrWhiteSpace(city)) City = city;
        if (!string.IsNullOrWhiteSpace(address)) Address = address;
        if (!string.IsNullOrWhiteSpace(postalCode)) PostalCode = postalCode;
        if (!string.IsNullOrWhiteSpace(phoneNumber)) PhoneNumber = phoneNumber;
        if (!string.IsNullOrWhiteSpace(mobileNumber)) MobileNumber = mobileNumber;
    }
}