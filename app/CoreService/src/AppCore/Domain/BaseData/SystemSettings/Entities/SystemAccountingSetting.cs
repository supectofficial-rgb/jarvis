namespace Insurance.AppCore.Domain.BaseData.SystemSettings.Entities;

using Insurance.AppCore.Domain.BaseData.SystemSettings.Enums;

/// <summary>
/// تنظیمات سیستم - حسابداری
/// </summary>
public sealed class SystemAccountingSetting
{
    public string? CompanyName { get; private set; }
    public string? NationalId { get; private set; }
    public string? EconomicCode { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public string? ProvinceName { get; private set; }
    public string? CityName { get; private set; }
    public string? PostalCode { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? FaxNumber { get; private set; }
    public string? Address { get; private set; }
    public string? Website { get; private set; }
    public string? Email { get; private set; }
    public InvoiceNumberTemplateType InvoiceNumberTemplateType { get; private set; }
    public int TaxRate { get; private set; } // 5 to 50 percent
}