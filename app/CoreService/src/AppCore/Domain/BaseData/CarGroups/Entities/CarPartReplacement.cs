namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using Insurance.AppCore.Domain.BaseData.CarGroups.Enums;
using System;

/// <summary>
/// قطعات جایگزین و سازگار
/// </summary>
public sealed class CarPartReplacement
{
    public long OriginalPartId { get; private set; }
    public long ReplacementPartId { get; private set; }
    public ReplacementType ReplacementType { get; private set; }
    public decimal CompatibilityScore { get; private set; } // 0-100
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public CarPart? OriginalPart { get; private set; }
    public CarPart? ReplacementPart { get; private set; }
}

