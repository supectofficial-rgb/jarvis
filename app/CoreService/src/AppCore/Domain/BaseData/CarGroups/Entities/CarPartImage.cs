namespace Insurance.AppCore.Domain.BaseData.CarGroups.Entities;

using Insurance.AppCore.Domain.BaseData.CarGroups.Enums;
using System;

/// <summary>
/// تصاویر قطعات خودرو
/// </summary>
public sealed class CarPartImage
{
    public long CarPartId { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public string? Description { get; private set; }
    public ImageType ImageType { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public string? CreatedByUsername { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public CarPart? CarPart { get; private set; }
}

