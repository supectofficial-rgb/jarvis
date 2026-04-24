namespace Insurance.DynamicStructureService.AppCore.Domain.FormTypes.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public class FormType : AggregateRoot
{
    public string Title { get; private set; } = null!;

    protected FormType() { }

    public static FormType Create(string title)
    {
        return new FormType { Title = title };
    }

    public void Update(string title)
    {
        Title = title;
    }
}

