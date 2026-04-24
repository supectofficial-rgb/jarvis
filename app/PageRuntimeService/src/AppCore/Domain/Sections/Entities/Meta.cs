using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Sections.Entities;

public class Meta : Entity<long>
{
    public string Name { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public long SectionId { get; private set; }

    protected Meta() { }

    public static Meta Create(string name, string content, long sectionId)
    {
        return new Meta
        {
            Name = name,
            Content = content,
            SectionId = sectionId
        };
    }

    public void Update(string name, string content, long sectionId)
    {
        Name = name;
        Content = content;
        SectionId = sectionId;
    }
}


