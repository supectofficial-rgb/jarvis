namespace Insurance.AppCore.Shared.Losts.Commands.AssignThirdPartyInsurerCode;

using Insurance.AppCore.Domain.Losts.ValueObjects;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands;
using System;

/// <summary>
/// صدور کد بیمه گران طرف قرارداد ثالث مالی
/// </summary>
public class AssignThirdPartyInsurerCodeCommand : ICommand<Guid>
{
    public Guid ProvinceBusinessKey { get; set; }
    public Guid CityBusinessKey { get; set; }
    /// <summary>
    /// محل بازدید
    /// </summary>
    public string? InspectionAddress { get; set; }
    /// <summary>
    /// محل حادثه
    /// </summary>
    public Location AccidentLocation { get; set; }
    /// <summary>
    /// تاریخ و زمان وقوع حادثه
    /// </summary>
    public DateTime? AccidentDateTime { get; set; }

    /// <summary>
    /// ارزیاب
    /// </summary>
    public BusinessKey AppraiserBusinessKey { get; set; }
    /// <summary>
    /// شعبه
    /// </summary>
    public string? Branch { get; set; }
}