namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using System;

/// <summary>
/// گروه خودرو
/// </summary>
public sealed class CarGroup
{
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDefault { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// کدینگ در سیستم بیمه‌گر
    /// </summary>
    public string? InsuranceCompanyCode { get; private set; }

    // Navigation properties
    public List<CarModel>? CarModels { get; private set; }
    public List<InsuranceCompanyGroupMapping>? InsuranceCompanyMappings { get; private set; }
}