using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Contents.Entities;

public class Content : Entity<long>
{
    public long? SectionTypeId { get; private set; }
    public int Count { get; private set; }
    public int Priority { get; private set; }
    public int UseParentSection { get; private set; }
    public int Pagination { get; private set; }
    public string CreateCategoryList { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string CycleFields { get; private set; } = null!;
    public string CycleFormItem { get; private set; } = null!;
    public string StackWeight { get; private set; } = null!;
    public string? FormId { get; private set; }
    public long? HtmlId { get; private set; }
    public long? SectionId { get; private set; }
    public long? ParentId { get; private set; }

    protected Content() { }

    public static Content Create(
        long? sectionTypeId,
        int count,
        int priority,
        int useParentSection,
        int pagination,
        string createCategoryList,
        string title,
        string description,
        string cycleFields,
        string cycleFormItem,
        string stackWeight,
        string? formId,
        long? htmlId,
        long? sectionId,
        long? parentId)
    {
        return new Content
        {
            SectionTypeId = sectionTypeId,
            Count = count,
            Priority = priority,
            UseParentSection = useParentSection,
            Pagination = pagination,
            CreateCategoryList = createCategoryList,
            Title = title,
            Description = description,
            CycleFields = cycleFields,
            CycleFormItem = cycleFormItem,
            StackWeight = stackWeight,
            FormId = formId,
            HtmlId = htmlId,
            SectionId = sectionId,
            ParentId = parentId
        };
    }

    public void Update(
        long? sectionTypeId,
        int count,
        int priority,
        int useParentSection,
        int pagination,
        string createCategoryList,
        string title,
        string description,
        string cycleFields,
        string cycleFormItem,
        string stackWeight,
        string? formId,
        long? htmlId,
        long? sectionId,
        long? parentId)
    {
        SectionTypeId = sectionTypeId;
        Count = count;
        Priority = priority;
        UseParentSection = useParentSection;
        Pagination = pagination;
        CreateCategoryList = createCategoryList;
        Title = title;
        Description = description;
        CycleFields = cycleFields;
        CycleFormItem = cycleFormItem;
        StackWeight = stackWeight;
        FormId = formId;
        HtmlId = htmlId;
        SectionId = sectionId;
        ParentId = parentId;
    }
}


