namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using Insurance.AppCore.Domain.BaseData.CarGroups.Enums;
using System;

/// <summary>
/// واریانت خودرو (انواع مختلف یک تریپ)
/// </summary>
public sealed class CarVariant
{
    public long CarTrimId { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string? VariantCode { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// کدینگ در سیستم بیمه‌گر
    /// </summary>
    public string? InsuranceCompanyCode { get; private set; }

    /// <summary>
    /// نوع صندوق
    /// </summary>
    public BodyType BodyType { get; private set; }

    /// <summary>
    /// تعداد درب
    /// </summary>
    public byte DoorCount { get; private set; }

    /// <summary>
    /// تعداد سرنشین
    /// </summary>
    public byte SeatCount { get; private set; }

    /// <summary>
    /// وزن خالص (کیلوگرم)
    /// </summary>
    public int CurbWeight { get; private set; }

    /// <summary>
    /// قیمت پایه (ریال)
    /// </summary>
    public decimal BasePrice { get; private set; }

    // Navigation properties
    public CarTrim? CarTrim { get; private set; }
    public List<CarPart>? CarParts { get; private set; }
    public List<CarVariantOption>? Options { get; private set; }
}
