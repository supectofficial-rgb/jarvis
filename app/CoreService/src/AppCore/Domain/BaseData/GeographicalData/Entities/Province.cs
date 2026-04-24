namespace Insurance.AppCore.Domain.BaseData.GeographicalData.Entities;

using Insurance.AppCore.Domain.BaseData.GeographicalData.Enums;
using System.Collections.Generic;

/// <summary>
/// اطلاعات استان
/// </summary>
public class Province
{
    /// <summary>
    /// شناسه استان
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// شناسه کشور
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// نام فارسی استان
    /// </summary>
    public string PersianName { get; set; } = string.Empty;

    /// <summary>
    /// نام انگلیسی استان
    /// </summary>
    public string EnglishName { get; set; } = string.Empty;

    /// <summary>
    /// مرکز استان
    /// </summary>
    public string Capital { get; set; } = string.Empty;

    /// <summary>
    /// کد استان (مثل 021 برای تهران)
    /// </summary>
    public string AreaCode { get; set; } = string.Empty;

    /// <summary>
    /// کد پستی استان (پیشوند)
    /// </summary>
    public string PostalCodePrefix { get; set; } = string.Empty;

    /// <summary>
    /// کد شماره‌گذاری پلاک خودرو
    /// </summary>
    public string VehiclePlateCode { get; set; } = string.Empty;

    /// <summary>
    /// جمعیت استان
    /// </summary>
    public long Population { get; set; }

    /// <summary>
    /// مساحت به کیلومتر مربع
    /// </summary>
    public double AreaSquareKm { get; set; }

    /// <summary>
    /// تعداد شهرستان‌ها
    /// </summary>
    public int CountiesCount { get; set; }

    /// <summary>
    /// وضعیت فعال بودن
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// مختصات جغرافیایی مرکز استان
    /// </summary>
    public GeoLocation GeoLocation { get; set; }

    /// <summary>
    /// اطلاعات کشور
    /// </summary>
    public Country Country { get; set; }

    /// <summary>
    /// لیست شهرهای استان
    /// </summary>
    public List<City> Cities { get; set; } = new List<City>();
}