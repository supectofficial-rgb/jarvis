using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.SectionLayouts.Entities;

public class LayoutPart : Entity<long>
{
    public string Title { get; private set; } = null!;
    public string TypeName { get; private set; } = null!;
    public long LanguageId { get; private set; }
    public long? SectionLayoutId { get; private set; }

    protected LayoutPart() { }

    public static LayoutPart Create(string title, string typeName, long languageId, long? sectionLayoutId)
    {
        return new LayoutPart
        {
            Title = title,
            TypeName = typeName,
            LanguageId = languageId,
            SectionLayoutId = sectionLayoutId
        };
    }

    public void Update(string title, string typeName, long languageId, long? sectionLayoutId)
    {
        Title = title;
        TypeName = typeName;
        LanguageId = languageId;
        SectionLayoutId = sectionLayoutId;
    }
}


