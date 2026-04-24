using System.ComponentModel;

namespace Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Enums;

/// <summary>
/// نوع پلاک
/// </summary>
public enum VehiclePlateType : byte
{
    None = 0,
    [Description("شخصی")]
    Private = 1,

    [Description("آژانس")]
    Agency = 2,

    [Description("آمبولانس")]
    Ambulance = 3,

    [Description("آموزشی")]
    Educational = 4,

    [Description("اداری")]
    Administrative = 5,

    [Description("امور شرکت")]
    CorporateAffairs = 6,

    [Description("تاکسی")]
    Taxi = 7,

    [Description("تاکسی (داخل و خارج شهر)")]
    TaxiUrbanSuburban = 8,

    [Description("تاکسی بیسیم")]
    RadioTaxi = 9,

    [Description("تاکسی خطی")]
    LineTaxi = 10,

    [Description("تاکسی کیش (ویژه قرارداد 1322)")]
    KishTaxi = 11,

    [Description("تشریفات")]
    Ceremonial = 12,

    [Description("تعلیم رانندگی")]
    DrivingSchool = 13,

    [Description("حمل پول")]
    MoneyTransport = 14,

    [Description("حمل جنازه")]
    Hearse = 15,

    [Description("حمل خون")]
    BloodTransport = 16,

    [Description("دولتی")]
    Governmental = 17,

    [Description("سرویس")]
    Service = 18,

    [Description("سیاسی")]
    Political = 19,

    [Description("سیاسی 2")]
    Political2 = 20,

    [Description("شخصی منطقه آزاد")]
    PrivateFreeZone = 21,

    [Description("فاقد پلاک")]
    WithoutPlate = 22,

    [Description("کرایه بیابانی")]
    DesertRental = 23,

    [Description("کرایه شهری")]
    UrbanRental = 24,

    [Description("مسافربر")]
    PassengerCarrier = 25,

    [Description("نظامی")]
    Military = 26
}