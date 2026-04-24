using OysterFx.AppCore.Domain.BusinessRules;

namespace Insurance.AppCore.Domain.BaseData.VehiclePlates.BusinessRules;

/// <summary>
/// قانون اعتبارسنجی حرف پلاک
/// </summary>
public class PlateLetterMustBeValidRule : IBusinessRule
{
    private readonly string _letter;
    private static readonly HashSet<string> ValidLetters = new()
        {
            "آ", "ا", "ب", "پ", "ت", "ث", "ج", "چ", "ح", "خ", "د", "ذ", "ر", "ز", "ژ",
            "س", "ش", "ص", "ض", "ط", "ظ", "ع", "غ", "ف", "ق", "ک", "گ", "ل", "م", "ن",
            "و", "ه", "ی"
        };

    public PlateLetterMustBeValidRule(string letter)
    {
        _letter = letter;
    }

    public string Message => "حرف پلاک نامعتبر است. لطفاً از حروف مجاز پلاک استفاده کنید";

    public bool IsBroken()
    {
        return string.IsNullOrEmpty(_letter) || !ValidLetters.Contains(_letter);
    }
}