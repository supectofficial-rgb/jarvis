using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Contents.Entities;

public class Pose : Entity<long>
{
    public string Title { get; private set; } = null!;
    public string Title2 { get; private set; } = null!;
    public long? ContentId { get; private set; }

    protected Pose() { }

    public static Pose Create(string title, string title2, long? contentId)
    {
        return new Pose
        {
            Title = title,
            Title2 = title2,
            ContentId = contentId
        };
    }

    public void Update(string title, string title2, long? contentId)
    {
        Title = title;
        Title2 = title2;
        ContentId = contentId;
    }
}


