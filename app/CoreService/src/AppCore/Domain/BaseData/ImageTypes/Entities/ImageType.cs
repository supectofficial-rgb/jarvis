namespace Insurance.AppCore.Domain.BaseData.ImageTypes.Entities;

public sealed class ImageType
{
    /// <summary>
    /// نام تصویر
    /// </summary>
    public string? ImageName { get; private set; }
    /// <summary>
    /// بخش
    /// </summary>
    public string? SectionName { get; private set; }
    /// <summary>
    /// اندازه
    /// </summary>
    public string? Size { get; private set; }
    /// <summary>                                     
    /// نحوه چاپ                                      
    /// </summary>
    public string? PrintType { get; private set; }
    /// <summary>                                      
    /// اضافه شده توسط                                      
    /// </summary>
    public int? CreatedById { get; private set; }
    /// <summary>                               
    /// اضافه شده توسط                               
    /// </summary>
    public string? CreatedByUserName { get; private set; }

    public ImageType()
    {
    }
}