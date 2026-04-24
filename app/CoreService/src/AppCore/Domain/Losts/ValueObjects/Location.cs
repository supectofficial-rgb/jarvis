namespace Insurance.AppCore.Domain.Losts.ValueObjects;

using Insurance.AppCore.Domain.Losts.BusinessRules;
using OysterFx.AppCore.Domain.ValueObjects;
using System;
using System.Collections.Generic;

public class Location : ValueObject
{
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public string? Name { get; private set; }
    public string? FullAddress { get; private set; }

    private Location() { }

    public Location(decimal latitude, decimal longitude, string? name = null, string? fullAddress = null)
    {
        ValidateCoordinates(latitude, longitude);

        Latitude = latitude;
        Longitude = longitude;
        Name = name;
        FullAddress = fullAddress;
    }

    /// <summary>
    /// ایجاد Location از مختصات
    /// </summary>
    public static Location Create(decimal latitude, decimal longitude, string? name = null, string? fullAddress = null)
    {
        return new Location(latitude, longitude, name, fullAddress);
    }

    /// <summary>
    /// به‌روزرسانی مختصات
    /// </summary>
    public Location UpdateCoordinates(decimal latitude, decimal longitude)
    {
        ValidateCoordinates(latitude, longitude);

        return new Location(latitude, longitude, Name, FullAddress);
    }

    /// <summary>
    /// به‌روزرسانی نام و آدرس
    /// </summary>
    public Location UpdateDetails(string? name, string? fullAddress)
    {
        return new Location(Latitude, Longitude, name, fullAddress);
    }

    /// <summary>
    /// محاسبه فاصله از یک مکان دیگر (فرمول ساده - در صورت نیاز می‌توانید پیاده‌سازی دقیق‌تر کنید)
    /// </summary>
    public decimal CalculateDistance(Location otherLocation)
    {
        // پیاده‌سازی ساده فاصله اقلیدسی
        // برای دقت بیشتر از فرمول Haversine استفاده کنید
        var latDiff = Latitude - otherLocation.Latitude;
        var lonDiff = Longitude - otherLocation.Longitude;

        return (decimal)Math.Sqrt((double)(latDiff * latDiff + lonDiff * lonDiff));
    }

    /// <summary>
    /// اعتبارسنجی مختصات
    /// </summary>
    private void ValidateCoordinates(decimal latitude, decimal longitude)
    {
        // مقادیر مجاز مختصات جغرافیایی
        CheckRule(new LocationCoordinatesMustBeValidRule(latitude, longitude));
    }

    /// <summary>
    /// بررسی یکسان بودن مختصات با تلرانس مشخص
    /// </summary>
    public bool CoordinatesEqual(Location other, decimal tolerance = 0.00001m)
    {
        return Math.Abs(Latitude - other.Latitude) <= tolerance &&
               Math.Abs(Longitude - other.Longitude) <= tolerance;
    }

    /// <summary>
    /// دریافت مختصات به صورت رشته فرمت‌شده
    /// </summary>
    public override string ToString()
    {
        return $"{Latitude:F6}, {Longitude:F6}" +
               (string.IsNullOrEmpty(Name) ? "" : $" ({Name})");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        // در مقایسه value object ها، تنها مختصات جغرافیایی مهم است
        // نام و آدرس جزو ویژگی‌های توصیفی هستند و در شناسایی یکتا تأثیری ندارند
        yield return Latitude;
        yield return Longitude;
    }

    /// <summary>
    /// ایجاد کپی از آبجکت
    /// </summary>
    public Location Copy()
    {
        return new Location(Latitude, Longitude, Name, FullAddress);
    }
}