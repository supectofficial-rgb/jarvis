namespace Insurance.DynamicStructureService.AppCore.Domain.Forms.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public class Form : AggregateRoot
{
    public int IsForThird { get; private set; }
    public int ThirdIdMainFlowId { get; private set; }
    public int SubmitLimitationMinute { get; private set; }
    public string RelationCondition { get; private set; } = null!;
    public string AuthMethod { get; private set; } = null!;
    public string ListUrl { get; private set; } = null!;
    public string CustomData { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string ZaribWidth { get; private set; } = null!;
    public string ZaribHeight { get; private set; } = null!;
    public string PdfBase { get; private set; } = null!;
    public string Pdf { get; private set; } = null!;
    public string Image { get; private set; } = null!;
    public string ImageWidth { get; private set; } = null!;
    public string ImageHeight { get; private set; } = null!;
    public int Priority { get; private set; }
    public string ProcessName { get; private set; } = null!;

    public long? UserId { get; private set; }
    public long? FormTypeId { get; private set; }
    public long? FormParentId { get; private set; }
    public long? ProcessId { get; private set; }

    private readonly List<FormItem> _items = new();
    public IReadOnlyCollection<FormItem> Items => _items.AsReadOnly();

    protected Form() { }

    public static Form Create(
        string title,
        string listUrl,
        string customData,
        string relationCondition,
        string authMethod,
        int isForThird,
        int thirdIdMainFlowId,
        int submitLimitationMinute,
        string zaribWidth,
        string zaribHeight,
        string pdfBase,
        string pdf,
        string image,
        string imageWidth,
        string imageHeight,
        int priority,
        string processName,
        long? userId,
        long? formTypeId,
        long? formParentId,
        long? processId)
    {
        return new Form
        {
            Title = title,
            ListUrl = listUrl,
            CustomData = customData,
            RelationCondition = relationCondition,
            AuthMethod = authMethod,
            IsForThird = isForThird,
            ThirdIdMainFlowId = thirdIdMainFlowId,
            SubmitLimitationMinute = submitLimitationMinute,
            ZaribWidth = zaribWidth,
            ZaribHeight = zaribHeight,
            PdfBase = pdfBase,
            Pdf = pdf,
            Image = image,
            ImageWidth = imageWidth,
            ImageHeight = imageHeight,
            Priority = priority,
            ProcessName = processName,
            UserId = userId,
            FormTypeId = formTypeId,
            FormParentId = formParentId,
            ProcessId = processId
        };
    }

    public void AddItem(FormItem item)
    {
        _items.Add(item);
    }

    public void Update(
        string title,
        string listUrl,
        string customData,
        string relationCondition,
        string authMethod,
        int isForThird,
        int thirdIdMainFlowId,
        int submitLimitationMinute,
        string zaribWidth,
        string zaribHeight,
        string pdfBase,
        string pdf,
        string image,
        string imageWidth,
        string imageHeight,
        int priority,
        string processName,
        long? userId,
        long? formTypeId,
        long? formParentId,
        long? processId)
    {
        Title = title;
        ListUrl = listUrl;
        CustomData = customData;
        RelationCondition = relationCondition;
        AuthMethod = authMethod;
        IsForThird = isForThird;
        ThirdIdMainFlowId = thirdIdMainFlowId;
        SubmitLimitationMinute = submitLimitationMinute;
        ZaribWidth = zaribWidth;
        ZaribHeight = zaribHeight;
        PdfBase = pdfBase;
        Pdf = pdf;
        Image = image;
        ImageWidth = imageWidth;
        ImageHeight = imageHeight;
        Priority = priority;
        ProcessName = processName;
        UserId = userId;
        FormTypeId = formTypeId;
        FormParentId = formParentId;
        ProcessId = processId;
    }
}

