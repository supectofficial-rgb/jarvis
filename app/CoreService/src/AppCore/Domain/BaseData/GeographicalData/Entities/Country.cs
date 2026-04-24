namespace Insurance.AppCore.Domain.BaseData.GeographicalData.Entities;

using Insurance.AppCore.Domain.BaseData.GeographicalData.Enums;
using System;
using System.Collections.Generic;

/// <summary>
/// اطلاعات کشور
/// </summary>
public class Country
{
    /// <summary>
    /// شناسه کشور
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// نام فارسی کشور
    /// </summary>
    public string PersianName { get; set; } = string.Empty;

    /// <summary>
    /// نام انگلیسی کشور
    /// </summary>
    public string EnglishName { get; set; } = string.Empty;

    /// <summary>
    /// کد ISO 3166-1 alpha-2 (مثل IR برای ایران)
    /// </summary>
    public string IsoAlpha2Code { get; set; } = string.Empty;

    /// <summary>
    /// کد ISO 3166-1 alpha-3 (مثل IRN برای ایران)
    /// </summary>
    public string IsoAlpha3Code { get; set; } = string.Empty;

    /// <summary>
    /// کد ISO 3166-1 عددی (مثل 364 برای ایران)
    /// </summary>
    public int IsoNumericCode { get; set; }

    /// <summary>
    /// پیش‌شماره تلفن (مثل +98 برای ایران)
    /// </summary>
    public string PhonePrefix { get; set; } = string.Empty;

    /// <summary>
    /// واحد پول (مثل ریال)
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// کد واحد پول (مثل IRR)
    /// </summary>
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// پایتخت کشور
    /// </summary>
    public string Capital { get; set; } = string.Empty;

    /// <summary>
    /// جمعیت کشور
    /// </summary>
    public long Population { get; set; }

    /// <summary>
    /// مساحت به کیلومتر مربع
    /// </summary>
    public double AreaSquareKm { get; set; }

    /// <summary>
    /// زبان رسمی
    /// </summary>
    public string OfficialLanguage { get; set; } = string.Empty;

    /// <summary>
    /// وضعیت فعال بودن
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// لیست استان‌های کشور
    /// </summary>
    public List<Province> Provinces { get; set; } = new List<Province>();

    /// <summary>
    /// مختصات جغرافیایی مرکز کشور
    /// </summary>
    public GeoLocation GeoLocation { get; set; }

    /// <summary>
    /// مناطق زمانی کشور
    /// </summary>
    public List<TimeZoneInfo> TimeZones { get; set; } = new List<TimeZoneInfo>();
}