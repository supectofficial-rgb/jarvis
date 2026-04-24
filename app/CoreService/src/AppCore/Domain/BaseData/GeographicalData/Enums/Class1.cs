using Insurance.AppCore.Domain.BaseData.GeographicalData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Insurance.AppCore.Domain.BaseData.GeographicalData.Enums;

internal class Class1
{
}

/// <summary>
/// مختصات جغرافیایی
/// </summary>
public class GeoLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Altitude { get; set; }
}

/// <summary>
/// منطقه شهری
/// </summary>
public class District
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public GeoLocation GeoLocation { get; set; }
    public City City { get; set; }
}

/// <summary>
/// نوع شهر
/// </summary>
public enum CityType : byte
{
    Unknown = 0,
    Capital = 1,           // مرکز استان
    Metropolis = 2,        // کلانشهر
    MajorCity = 3,         // شهر بزرگ
    MediumCity = 4,        // شهر متوسط
    SmallCity = 5,         // شهر کوچک
    Town = 6,              // شهرک
    Village = 7            // روستا
}

/// <summary>
/// نوع آب و هوا
/// </summary>
public enum ClimateType : byte
{
    Unknown = 0,
    Desert = 1,            // بیابانی
    SemiDesert = 2,        // نیمه‌بیابانی
    Mediterranean = 3,     // مدیترانه‌ای
    Mountain = 4,          // کوهستانی
    Coastal = 5,           // ساحلی
    Continental = 6,       // قاره‌ای
    Steppe = 7             // استپی
}