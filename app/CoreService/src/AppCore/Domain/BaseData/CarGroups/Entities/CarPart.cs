namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using Insurance.AppCore.Domain.BaseData.CarGroups.Enums;

/// <summary>
/// قطعات خودرو
/// </summary>
public sealed class CarPart
{
    public long CarVariantId { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string? PartCode { get; private set; }
    public string? OEMCode { get; private set; }
    public CarPartCategory Category { get; private set; }
    public decimal StandardPrice { get; private set; }
    public decimal OEMPrice { get; private set; }
    public decimal AftermarketPrice { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public CarVariant? CarVariant { get; private set; }
    public List<CarPartReplacement>? Replacements { get; private set; }
}
