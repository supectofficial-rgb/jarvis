using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Contents.Entities;

public class Data : Entity<long>
{
    public string Title { get; private set; } = null!;
    public DateTime Date { get; private set; }
    public string Writer { get; private set; } = null!;
    public string Title2 { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Description2 { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public int Priority { get; private set; }
    public string MediaUrl { get; private set; } = null!;
    public string VideoIframe { get; private set; } = null!;
    public long? ContentId { get; private set; }

    protected Data() { }

    public static Data Create(
        string title,
        DateTime date,
        string writer,
        string title2,
        string description,
        string description2,
        string url,
        int priority,
        string mediaUrl,
        string videoIframe,
        long? contentId)
    {
        return new Data
        {
            Title = title,
            Date = date,
            Writer = writer,
            Title2 = title2,
            Description = description,
            Description2 = description2,
            Url = url,
            Priority = priority,
            MediaUrl = mediaUrl,
            VideoIframe = videoIframe,
            ContentId = contentId
        };
    }

    public void Update(
        string title,
        DateTime date,
        string writer,
        string title2,
        string description,
        string description2,
        string url,
        int priority,
        string mediaUrl,
        string videoIframe,
        long? contentId)
    {
        Title = title;
        Date = date;
        Writer = writer;
        Title2 = title2;
        Description = description;
        Description2 = description2;
        Url = url;
        Priority = priority;
        MediaUrl = mediaUrl;
        VideoIframe = videoIframe;
        ContentId = contentId;
    }
}


