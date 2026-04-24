namespace Insurance.UserService.Infra.Persistence.RDB.Queries.Organizations.Entities;

using System;

public class Organization
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string? TenantId { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
}