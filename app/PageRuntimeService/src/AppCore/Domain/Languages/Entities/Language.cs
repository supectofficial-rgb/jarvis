using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Languages.Entities;

public class Language : AggregateRoot
{
    public string Title { get; private set; } = null!;

    protected Language() { }

    public static Language Create(string title)
    {
        return new Language { Title = title };
    }

    public void Update(string title)
    {
        Title = title;
    }
}


