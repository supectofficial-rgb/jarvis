namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using System;

/// <summary>
/// جدول تعیین قیمت قطعات
/// </summary>
public sealed class CarPartPricing
{
    public long CarPartId { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public decimal OEMPrice { get; private set; }
    public decimal AftermarketPrice { get; private set; }
    public decimal UsedPrice { get; private set; }
    public decimal RepairCostMin { get; private set; }
    public decimal RepairCostMax { get; private set; }
    public decimal LaborCost { get; private set; }
    public string? PricingSource { get; private set; }
    public bool IsActive { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public CarPart? CarPart { get; private set; }
}

