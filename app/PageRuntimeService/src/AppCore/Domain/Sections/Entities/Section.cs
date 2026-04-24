using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Sections.Entities;

public class Section : AggregateRoot
{
    public string Url { get; private set; } = null!;
    public string Closed { get; private set; } = null!;
    public string ButtonText { get; private set; } = null!;
    public DateTime Date { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string MetaData { get; private set; } = null!;
    public string Schema { get; private set; } = null!;
    public string MetaTitle { get; private set; } = null!;
    public string Image { get; private set; } = null!;
    public string Writer { get; private set; } = null!;

    public long? CategoryId { get; private set; }
    public long LanguageId { get; private set; }
    public long? SectionLayoutId { get; private set; }
    public long? SectionTypeId { get; private set; }

    protected Section() { }

    public static Section Create(
        string url,
        string title,
        string description,
        string metaData,
        string schema,
        string metaTitle,
        string image,
        string writer,
        string closed,
        string buttonText,
        DateTime date,
        long languageId,
        long? sectionTypeId,
        long? sectionLayoutId,
        long? categoryId)
    {
        return new Section
        {
            Url = url,
            Title = title,
            Description = description,
            MetaData = metaData,
            Schema = schema,
            MetaTitle = metaTitle,
            Image = image,
            Writer = writer,
            Closed = closed,
            ButtonText = buttonText,
            Date = date,
            LanguageId = languageId,
            SectionTypeId = sectionTypeId,
            SectionLayoutId = sectionLayoutId,
            CategoryId = categoryId
        };
    }

    public void Update(
        string url,
        string title,
        string description,
        string metaData,
        string schema,
        string metaTitle,
        string image,
        string writer,
        string closed,
        string buttonText,
        DateTime date,
        long languageId,
        long? sectionTypeId,
        long? sectionLayoutId,
        long? categoryId)
    {
        Url = url;
        Title = title;
        Description = description;
        MetaData = metaData;
        Schema = schema;
        MetaTitle = metaTitle;
        Image = image;
        Writer = writer;
        Closed = closed;
        ButtonText = buttonText;
        Date = date;
        LanguageId = languageId;
        SectionTypeId = sectionTypeId;
        SectionLayoutId = sectionLayoutId;
        CategoryId = categoryId;
    }
}


