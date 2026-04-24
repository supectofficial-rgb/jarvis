namespace Insurance.DynamicStructureService.AppCore.Domain.FormItemTypes.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public class FormItemType : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public string FormItemTypeCode { get; private set; } = null!;
    public long? UserId { get; private set; }

    protected FormItemType() { }

    public static FormItemType Create(string title, string formItemTypeCode, long? userId)
    {
        return new FormItemType
        {
            Title = title,
            FormItemTypeCode = formItemTypeCode,
            UserId = userId
        };
    }
}

