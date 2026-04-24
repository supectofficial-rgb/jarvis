namespace Insurance.AppCore.Domain.BaseData.GeographicalData.Helpers;

using Insurance.AppCore.Domain.BaseData.GeographicalData.Entities;
using Insurance.AppCore.Domain.BaseData.GeographicalData.Enums;


/// <summary>
/// اطلاعات ثابت کشور ایران
/// </summary>
public static class Iran
{
    public static Country GetCountryInfo()
    {
        return new Country
        {
            Id = 1,
            PersianName = "ایران",
            EnglishName = "Iran",
            IsoAlpha2Code = "IR",
            IsoAlpha3Code = "IRN",
            IsoNumericCode = 364,
            PhonePrefix = "+98",
            Currency = "ریال",
            CurrencyCode = "IRR",
            Capital = "تهران",
            Population = 85000000,
            AreaSquareKm = 1648195,
            OfficialLanguage = "فارسی",
            IsActive = true,
            GeoLocation = new GeoLocation
            {
                Latitude = 32.4279,
                Longitude = 53.6880
            }
        };
    }

    /// <summary>
    /// لیست استان‌های ایران
    /// </summary>
    public static List<Province> GetProvinces()
    {
        return new List<Province>
        {
            new Province { Id = 1, PersianName = "تهران", EnglishName = "Tehran",
                         Capital = "تهران", AreaCode = "021", VehiclePlateCode = "11",
                         Population = 13200000, AreaSquareKm = 18814 },
            new Province { Id = 2, PersianName = "اصفهان", EnglishName = "Isfahan",
                         Capital = "اصفهان", AreaCode = "031", VehiclePlateCode = "12" },
            new Province { Id = 3, PersianName = "خراسان رضوی", EnglishName = "Razavi Khorasan",
                         Capital = "مشهد", AreaCode = "051", VehiclePlateCode = "42" },
            // سایر استان‌ها...
        };
    }
}