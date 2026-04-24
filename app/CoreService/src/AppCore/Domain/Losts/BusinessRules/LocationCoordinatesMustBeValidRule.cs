using OysterFx.AppCore.Domain.BusinessRules;

namespace Insurance.AppCore.Domain.Losts.BusinessRules;

/// <summary>
/// قانون کسب‌وکار برای اعتبارسنجی مختصات
/// </summary>
public class LocationCoordinatesMustBeValidRule : IBusinessRule
{
    private readonly decimal _latitude;
    private readonly decimal _longitude;

    public string Message => "مختصات جغرافیایی نامعتبر است. عرض جغرافیایی باید بین ۹۰- تا ۹۰+ و طول جغرافیایی باید بین ۱۸۰- تا ۱۸۰+ باشد.";

    public LocationCoordinatesMustBeValidRule(decimal latitude, decimal longitude)
    {
        _latitude = latitude;
        _longitude = longitude;
    }

    public bool IsBroken()
    {
        return _latitude < -90m || _latitude > 90m ||
               _longitude < -180m || _longitude > 180m;
    }
}