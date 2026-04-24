namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using System;

/// <summary>
/// مدل خودرو
/// </summary>
public sealed class CarModel
{
    public long CarGroupId { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// کدینگ در سیستم بیمه‌گر
    /// </summary>
    public string? InsuranceCompanyCode { get; private set; }

    // Navigation properties
    public CarGroup? CarGroup { get; private set; }
    public List<CarTrim>? CarTrims { get; private set; }
}
