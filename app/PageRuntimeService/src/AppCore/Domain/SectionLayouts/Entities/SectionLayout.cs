using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.SectionLayouts.Entities;

public class SectionLayout : AggregateRoot
{
    public string MenuTitle { get; private set; } = null!;
    public long? LayoutId { get; private set; }
    public long LanguageId { get; private set; }

    protected SectionLayout() { }

    public static SectionLayout Create(string menuTitle, long? layoutId, long languageId)
    {
        return new SectionLayout
        {
            MenuTitle = menuTitle,
            LayoutId = layoutId,
            LanguageId = languageId
        };
    }

    public void Update(string menuTitle, long? layoutId, long languageId)
    {
        MenuTitle = menuTitle;
        LayoutId = layoutId;
        LanguageId = languageId;
    }
}


