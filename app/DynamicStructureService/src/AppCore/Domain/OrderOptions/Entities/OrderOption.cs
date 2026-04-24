namespace Insurance.DynamicStructureService.AppCore.Domain.OrderOptions.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public class OrderOption : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public int Priority { get; private set; }
    public string Value { get; private set; } = null!;
    public string Image { get; private set; } = null!;
    public long? UserId { get; private set; }
    public long? ParentId { get; private set; }

    protected OrderOption() { }

    public static OrderOption Create(string title, int priority, string value, string image, long? userId, long? parentId)
    {
        return new OrderOption
        {
            Title = title,
            Priority = priority,
            Value = value,
            Image = image,
            UserId = userId,
            ParentId = parentId
        };
    }

    public void Update(string title, int priority, string value, string image, long? userId, long? parentId)
    {
        Title = title;
        Priority = priority;
        Value = value;
        Image = image;
        UserId = userId;
        ParentId = parentId;
    }
}

