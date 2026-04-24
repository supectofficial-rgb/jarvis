using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Layouts.Entities;

public class Layout : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Image { get; private set; } = null!;
    public string PartName { get; private set; } = null!;
    public string PartDetailName { get; private set; } = null!;

    protected Layout() { }

    public static Layout Create(string title, string name, string description, string image, string partName, string partDetailName)
    {
        return new Layout
        {
            Title = title,
            Name = name,
            Description = description,
            Image = image,
            PartName = partName,
            PartDetailName = partDetailName
        };
    }

    public void Update(string title, string name, string description, string image, string partName, string partDetailName)
    {
        Title = title;
        Name = name;
        Description = description;
        Image = image;
        PartName = partName;
        PartDetailName = partDetailName;
    }
}


