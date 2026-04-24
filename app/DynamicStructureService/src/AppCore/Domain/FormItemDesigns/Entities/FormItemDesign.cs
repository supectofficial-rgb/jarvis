namespace Insurance.DynamicStructureService.AppCore.Domain.FormItemDesigns.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public class FormItemDesign : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public int Number { get; private set; }

    protected FormItemDesign() { }

    public static FormItemDesign Create(string title, int number)
    {
        return new FormItemDesign
        {
            Title = title,
            Number = number
        };
    }
}

