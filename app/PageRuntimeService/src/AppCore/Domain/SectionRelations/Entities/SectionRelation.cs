using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.SectionRelations.Entities;

public class SectionRelation : AggregateRoot
{
    public string Url { get; private set; } = null!;
    public string RelationCode { get; private set; } = null!;

    protected SectionRelation() { }

    public static SectionRelation Create(string url, string relationCode)
    {
        return new SectionRelation
        {
            Url = url,
            RelationCode = relationCode
        };
    }

    public void Update(string url, string relationCode)
    {
        Url = url;
        RelationCode = relationCode;
    }
}


