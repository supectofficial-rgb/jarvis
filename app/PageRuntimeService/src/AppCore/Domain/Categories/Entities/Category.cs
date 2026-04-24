using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Categories.Entities;

public class Category : AggregateRoot
{
    public long? ParentCategoryId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Meta { get; private set; } = null!;
    public string Image { get; private set; } = null!;
    public long SectionTypeId { get; private set; }

    protected Category() { }

    public static Category Create(long? parentCategoryId, string title, string meta, string image, long sectionTypeId)
    {
        return new Category
        {
            ParentCategoryId = parentCategoryId,
            Title = title,
            Meta = meta,
            Image = image,
            SectionTypeId = sectionTypeId
        };
    }

    public void Update(long? parentCategoryId, string title, string meta, string image, long sectionTypeId)
    {
        ParentCategoryId = parentCategoryId;
        Title = title;
        Meta = meta;
        Image = image;
        SectionTypeId = sectionTypeId;
    }
}


