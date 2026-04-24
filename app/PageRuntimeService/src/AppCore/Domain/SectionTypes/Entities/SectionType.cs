using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.SectionTypes.Entities;

public class SectionType : AggregateRoot
{
    public long? FormId { get; private set; }
    public string CategoryDefaultHtml { get; private set; } = null!;
    public string AllowedHtmls { get; private set; } = null!;
    public string DefaultHtmls { get; private set; } = null!;
    public string Closed { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Meta { get; private set; } = null!;

    protected SectionType() { }

    public static SectionType Create(
        string title,
        string meta,
        string categoryDefaultHtml,
        string allowedHtmls,
        string defaultHtmls,
        string closed,
        long? formId)
    {
        return new SectionType
        {
            Title = title,
            Meta = meta,
            CategoryDefaultHtml = categoryDefaultHtml,
            AllowedHtmls = allowedHtmls,
            DefaultHtmls = defaultHtmls,
            Closed = closed,
            FormId = formId
        };
    }

    public void Update(
        string title,
        string meta,
        string categoryDefaultHtml,
        string allowedHtmls,
        string defaultHtmls,
        string closed,
        long? formId)
    {
        Title = title;
        Meta = meta;
        CategoryDefaultHtml = categoryDefaultHtml;
        AllowedHtmls = allowedHtmls;
        DefaultHtmls = defaultHtmls;
        Closed = closed;
        FormId = formId;
    }
}


