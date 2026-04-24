namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using Insurance.AppCore.Domain.BaseData.CarGroups.Enums;
using System;

/// <summary>
/// استانداردهای تعمیر قطعات
/// </summary>
public sealed class CarPartRepairStandard
{
    public long CarPartId { get; private set; }
    public RepairType RepairType { get; private set; }
    public decimal StandardTimeHours { get; private set; } // زمان استاندارد تعمیر
    public string? RepairProcedure { get; private set; }
    public string? RequiredTools { get; private set; }
    public string? SafetyNotes { get; private set; }
    public bool IsActive { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public CarPart? CarPart { get; private set; }
}

