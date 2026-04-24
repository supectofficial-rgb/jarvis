namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using Insurance.AppCore.Domain.BaseData.CarGroups.Enums;
using System;

/// <summary>
/// تریپ/آپشن خودرو
/// </summary>
public sealed class CarTrim
{
    public long CarModelId { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public int ProductionYear { get; private set; }
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
    /// ظرفیت موتور (سی‌سی)
    /// </summary>
    public int EngineCapacity { get; private set; }

    /// <summary>
    /// نوع سوخت
    /// </summary>
    public FuelType FuelType { get; private set; }

    /// <summary>
    /// نوع گیربکس
    /// </summary>
    public TransmissionType TransmissionType { get; private set; }

    // Navigation properties
    public CarModel? CarModel { get; private set; }
    public List<CarVariant>? CarVariants { get; private set; }
}