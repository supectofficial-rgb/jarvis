namespace Insurance.AppCore.Domain.BaseData.GeographicalData.Entities;

using Insurance.AppCore.Domain.BaseData.GeographicalData.Enums;
using System.Collections.Generic;

/// <summary>
/// اطلاعات شهر
/// </summary>
public class City
{
    /// <summary>
    /// شناسه شهر
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// شناسه استان
    /// </summary>
    public int ProvinceId { get; set; }

    /// <summary>
    /// نام فارسی شهر
    /// </summary>
    public string PersianName { get; set; } = string.Empty;

    /// <summary>
    /// نام انگلیسی شهر
    /// </summary>
    public string EnglishName { get; set; } = string.Empty;

    /// <summary>
    /// نام قدیم شهر
    /// </summary>
    public string HistoricalName { get; set; } = string.Empty;

    /// <summary>
    /// نوع شهر (مرکز استان، کلانشهر، شهر بزرگ، شهر متوسط، شهر کوچک)
    /// </summary>
    public CityType CityType { get; set; }

    /// <summary>
    /// جمعیت شهر
    /// </summary>
    public long Population { get; set; }

    /// <summary>
    /// ارتفاع از سطح دریا (متر)
    /// </summary>
    public int Elevation { get; set; }

    /// <summary>
    /// کد تلفن شهر
    /// </summary>
    public string CityCode { get; set; } = string.Empty;

    /// <summary>
    /// کد پستی شهر (پیشوند)
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// آب و هوای شهر
    /// </summary>
    public ClimateType Climate { get; set; }

    /// <summary>
    /// وضعیت فعال بودن
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// مختصات جغرافیایی شهر
    /// </summary>
    public GeoLocation GeoLocation { get; set; }

    /// <summary>
    /// اطلاعات استان
    /// </summary>
    public Province Province { get; set; }

    /// <summary>
    /// لیست محله‌ها یا مناطق شهر
    /// </summary>
    public List<District> Districts { get; set; } = new List<District>();
}