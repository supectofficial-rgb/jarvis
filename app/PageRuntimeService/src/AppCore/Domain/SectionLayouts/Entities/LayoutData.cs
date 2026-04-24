using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.SectionLayouts.Entities;

public class LayoutData : Entity<long>
{
    public string Title { get; private set; } = null!;
    public string UrlTitle { get; private set; } = null!;
    public long LayoutPartId { get; private set; }
    public long? SectionTypeId { get; private set; }
    public long? ParentId { get; private set; }
    public string DataType { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public int Priority { get; private set; }
    public string Image { get; private set; } = null!;
    public int TypeCount { get; private set; }

    protected LayoutData() { }

    public static LayoutData Create(
        string title,
        string urlTitle,
        long layoutPartId,
        long? sectionTypeId,
        long? parentId,
        string dataType,
        string url,
        int priority,
        string image,
        int typeCount)
    {
        return new LayoutData
        {
            Title = title,
            UrlTitle = urlTitle,
            LayoutPartId = layoutPartId,
            SectionTypeId = sectionTypeId,
            ParentId = parentId,
            DataType = dataType,
            Url = url,
            Priority = priority,
            Image = image,
            TypeCount = typeCount
        };
    }

    public void Update(
        string title,
        string urlTitle,
        long layoutPartId,
        long? sectionTypeId,
        long? parentId,
        string dataType,
        string url,
        int priority,
        string image,
        int typeCount)
    {
        Title = title;
        UrlTitle = urlTitle;
        LayoutPartId = layoutPartId;
        SectionTypeId = sectionTypeId;
        ParentId = parentId;
        DataType = dataType;
        Url = url;
        Priority = priority;
        Image = image;
        TypeCount = typeCount;
    }
}


