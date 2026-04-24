using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.SecTags.Entities;

public class SecTag : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public long SectionTypeId { get; private set; }

    protected SecTag() { }

    public static SecTag Create(string title, long sectionTypeId)
    {
        return new SecTag
        {
            Title = title,
            SectionTypeId = sectionTypeId
        };
    }

    public void Update(string title, long sectionTypeId)
    {
        Title = title;
        SectionTypeId = sectionTypeId;
    }
}


