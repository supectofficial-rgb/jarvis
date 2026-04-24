using OysterFx.AppCore.Domain.BusinessRules;

namespace Insurance.AppCore.Domain.BaseData.VehiclePlates.BusinessRules;

// ================ قوانین کسب‌وکار ================

/// <summary>
/// قانون اعتبارسنجی ارقام پلاک
/// </summary>
public class PlateDigitsMustBeValidRule : IBusinessRule
{
    private readonly string _digits;
    private readonly int _expectedLength;
    private readonly string _fieldName;

    public PlateDigitsMustBeValidRule(string digits, int expectedLength, string fieldName)
    {
        _digits = digits;
        _expectedLength = expectedLength;
        _fieldName = fieldName;
    }

    public string Message => $"{_fieldName} باید {_expectedLength} رقم باشد و تنها شامل اعداد ۰-۹ باشد";

    public bool IsBroken()
    {
        if (string.IsNullOrEmpty(_digits) || _digits.Length != _expectedLength)
            return true;

        foreach (var c in _digits)
        {
            if (!char.IsDigit(c))
                return true;
        }

        return false;
    }
}
