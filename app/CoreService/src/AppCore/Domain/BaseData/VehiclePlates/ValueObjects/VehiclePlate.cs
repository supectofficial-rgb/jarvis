using Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Enums;
using Insurance.AppCore.Domain.BaseData.VehiclePlates.BusinessRules;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Insurance.AppCore.Domain.BaseData.VehiclePlates.ValueObjects;

/// <summary>
/// پلاک وسیله نقلیه ایران
/// </summary>
public sealed class VehiclePlate : ValueObject
{
    /// <summary>
    /// دو رقم سمت راست (شماره سریال)
    /// </summary>
    public string FirstTwoDigits { get; private set; }

    /// <summary>
    /// حرف پلاک (الف، ب، پ، ت، ث، ج، چ، ح، خ، د، ذ، ر، ز، ژ، س، ش، ص، ض، ط، ظ، ع، غ، ف، ق، ک، گ، ل، م، ن، و، ه، ی)
    /// </summary>
    public string Letter { get; private set; }

    /// <summary>
    /// سه رقم سمت چپ (شماره منطقه/سریال)
    /// </summary>
    public string ThreeDigits { get; private set; }

    /// <summary>
    /// کد کشور (ایران: 12 و 18 و ...)
    /// </summary>
    public string? CountryCode { get; private set; }

    /// <summary>
    /// شماره پلاک به صورت کامل فرمت‌شده
    /// </summary>
    public string FullPlateNumber => ToString();

    /// <summary>
    /// نوع پلاک / کاربری خودرو
    /// </summary>
    public VehiclePlateType PlateType { get; private set; }

    /// <summary>
    /// استان صادرکننده پلاک
    /// </summary>
    public string? ProvinceCode { get; private set; }

    /// <summary>
    /// آیا پلاک دائمی است؟
    /// </summary>
    public bool IsPermanent { get; private set; }

    private VehiclePlate() { }

    private VehiclePlate(
        string firstTwoDigits,
        string letter,
        string threeDigits,
        VehiclePlateType plateType,
        string? countryCode = null,
        string? provinceCode = null,
        bool isPermanent = true)
    {
        FirstTwoDigits = firstTwoDigits;
        Letter = letter;
        ThreeDigits = threeDigits;
        PlateType = plateType;
        CountryCode = countryCode;
        ProvinceCode = provinceCode;
        IsPermanent = isPermanent;
    }

    /// <summary>
    /// ایجاد پلاک خودروی شخصی (فرمت: 12 الف 345)
    /// </summary>
    public static VehiclePlate CreatePrivatePlate(
        string firstTwoDigits,
        string letter,
        string threeDigits,
        string? provinceCode = null)
    {
        ValidateFirstTwoDigits(firstTwoDigits);
        ValidateLetter(letter);
        ValidateThreeDigits(threeDigits);
        ValidateProvinceCode(provinceCode);

        return new VehiclePlate(
            firstTwoDigits,
            letter,
            threeDigits,
            VehiclePlateType.Private,
            null,
            provinceCode,
            true);
    }

    /// <summary>
    /// ایجاد پلاک خودروی شخصی پلاک لیزری جدید
    /// </summary>
    public static VehiclePlate CreatePrivateNewLaserPlate(
        string firstTwoDigits,
        string letter,
        string threeDigits,
        string? provinceCode = null)
    {
        ValidateFirstTwoDigits(firstTwoDigits);
        ValidateLetter(letter);
        ValidateThreeDigits(threeDigits);
        ValidateProvinceCode(provinceCode);

        return new VehiclePlate(
            firstTwoDigits,
            letter,
            threeDigits,
            VehiclePlateType.Private,
            null,
            provinceCode,
            true);
    }

    /// <summary>
    /// ایجاد پلاک تاکسی
    /// </summary>
    public static VehiclePlate CreateTaxiPlate(
        string firstTwoDigits,
        string letter,
        string threeDigits,
        string? provinceCode = null)
    {
        ValidateFirstTwoDigits(firstTwoDigits);
        ValidateLetter(letter);
        ValidateThreeDigits(threeDigits);

        return new VehiclePlate(
            firstTwoDigits,
            letter,
            threeDigits,
            VehiclePlateType.Taxi,
            null,
            provinceCode,
            true);
    }

    /// <summary>
    /// ایجاد پلاک دولتی
    /// </summary>
    public static VehiclePlate CreateGovernmentalPlate(
        string firstTwoDigits,
        string letter,
        string threeDigits,
        string? provinceCode = null)
    {
        ValidateFirstTwoDigits(firstTwoDigits);
        ValidateLetter(letter);
        ValidateThreeDigits(threeDigits);

        return new VehiclePlate(
            firstTwoDigits,
            letter,
            threeDigits,
            VehiclePlateType.Governmental,
            null,
            provinceCode,
            true);
    }

    /// <summary>
    /// ایجاد پلاک نظامی
    /// </summary>
    public static VehiclePlate CreateMilitaryPlate(
        string firstTwoDigits,
        string letter,
        string threeDigits)
    {
        ValidateFirstTwoDigits(firstTwoDigits);
        ValidateLetter(letter);
        ValidateThreeDigits(threeDigits);

        return new VehiclePlate(
            firstTwoDigits,
            letter,
            threeDigits,
            VehiclePlateType.Military,
            null,
            null,
            true);
    }

    /// <summary>
    /// ایجاد پلاک از روی رشته کامل
    /// فرمت‌های پشتیبانی‌شده:
    /// - 12 الف 345
    /// - 12-الف-345
    /// - 12الف345
    /// - IR12الف345
    /// - 12 الف 345 ایران 18
    /// </summary>
    public static VehiclePlate Parse(string plateString, VehiclePlateType plateType = VehiclePlateType.Private)
    {
        if (string.IsNullOrWhiteSpace(plateString))
            throw new ArgumentException("شماره پلاک نمی‌تواند خالی باشد", nameof(plateString));

        // حذف فاصله‌های اضافی و نرمال‌سازی
        plateString = plateString.Trim().Replace(" ", "").Replace("-", "");

        // الگوی پلاک ایران: 2 رقم، 1 حرف، 3 رقم
        var pattern = @"^(\d{2})([آابپتثجچحخدذرزژسشصضطظعغفقکگلمنوهی])(\d{3})(?:.*)?$";
        var match = Regex.Match(plateString, pattern);

        if (!match.Success)
            throw new ArgumentException("فرمت شماره پلاک نامعتبر است", nameof(plateString));

        var firstTwoDigits = match.Groups[1].Value;
        var letter = match.Groups[2].Value;
        var threeDigits = match.Groups[3].Value;

        // استخراج کد کشور در صورت وجود
        string? countryCode = null;
        if (plateString.Contains("12") || plateString.Contains("18"))
        {
            var countryMatch = Regex.Match(plateString, @"(\d{2})$");
            if (countryMatch.Success)
                countryCode = countryMatch.Value;
        }

        return new VehiclePlate(
            firstTwoDigits,
            letter,
            threeDigits,
            plateType,
            countryCode,
            null,
            true);
    }

    /// <summary>
    /// بررسی معتبر بودن پلاک
    /// </summary>
    public bool IsValid()
    {
        try
        {
            ValidateFirstTwoDigits(FirstTwoDigits);
            ValidateLetter(Letter);
            ValidateThreeDigits(ThreeDigits);
            ValidateProvinceCode(ProvinceCode);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// دریافت فرمت استاندارد پلاک (12 الف 345)
    /// </summary>
    public string ToStandardFormat()
    {
        return $"{FirstTwoDigits} {Letter} {ThreeDigits}";
    }

    /// <summary>
    /// دریافت فرمت کامل پلاک با کد کشور
    /// </summary>
    public string ToFullFormat()
    {
        var baseFormat = ToStandardFormat();

        if (!string.IsNullOrEmpty(CountryCode))
            return $"{baseFormat} ایران {CountryCode}";

        if (!string.IsNullOrEmpty(ProvinceCode))
            return $"{baseFormat} - {ProvinceCode}";

        return baseFormat;
    }

    public override string ToString()
    {
        return ToStandardFormat();
    }

    // ================ اعتبارسنجی‌ها ================

    private static void ValidateFirstTwoDigits(string digits)
    {
        CheckRule(new PlateDigitsMustBeValidRule(digits, 2, "دو رقم اول"));
    }

    private static void ValidateThreeDigits(string digits)
    {
        CheckRule(new PlateDigitsMustBeValidRule(digits, 3, "سه رقم"));
    }

    private static void ValidateLetter(string letter)
    {
        CheckRule(new PlateLetterMustBeValidRule(letter));
    }

    private static void ValidateProvinceCode(string? provinceCode)
    {
        if (!string.IsNullOrEmpty(provinceCode))
            CheckRule(new PlateDigitsMustBeValidRule(provinceCode, 2, "کد استان"));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstTwoDigits;
        yield return Letter;
        yield return ThreeDigits;
        yield return PlateType;
        yield return CountryCode ?? string.Empty;
    }
}
