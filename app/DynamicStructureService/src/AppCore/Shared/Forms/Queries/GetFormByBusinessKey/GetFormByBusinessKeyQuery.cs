namespace Insurance.DynamicStructureService.AppCore.Shared.Forms.Queries.GetFormByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public sealed class GetFormByBusinessKeyQuery : IQuery<GetFormByBusinessKeyQueryResult>
{
    public GetFormByBusinessKeyQuery(Guid formBusinessKey)
    {
        FormBusinessKey = formBusinessKey;
    }

    public Guid FormBusinessKey { get; }
}

public sealed class GetFormByBusinessKeyQueryResult
{
    public Guid BusinessKey { get; set; }
    public string Title { get; set; } = string.Empty;
    public long? FormTypeId { get; set; }
    public long? UserId { get; set; }
}

