namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

/// <summary>
/// آپشن‌های واریانت خودرو
/// </summary>
public sealed class CarVariantOption
{
    public long CarVariantId { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string? OptionCode { get; private set; }
    public bool IsStandard { get; private set; }
    public decimal AdditionalPrice { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation property
    public CarVariant? CarVariant { get; private set; }
}
