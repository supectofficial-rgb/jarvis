namespace Insurance.DynamicStructureService.Infra.Persistence.RDB.Queries.DynamicStructures.Entities;

public sealed class FormReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string Title { get; set; } = string.Empty;
    public long? FormTypeId { get; set; }
    public long? UserId { get; set; }
}
