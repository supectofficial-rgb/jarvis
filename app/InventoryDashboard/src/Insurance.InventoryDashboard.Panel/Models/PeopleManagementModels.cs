namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class PeopleManagementSnapshot
{
    public IReadOnlyList<UserSummaryModel> Users { get; set; } = Array.Empty<UserSummaryModel>();
    public IReadOnlyList<PersonaSummaryModel> Personas { get; set; } = Array.Empty<PersonaSummaryModel>();
    public IReadOnlyList<PermissionSummaryModel> Permissions { get; set; } = Array.Empty<PermissionSummaryModel>();
    public string? ErrorMessage { get; set; }
}

public sealed class UserSummaryModel
{
    public long Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

public sealed class PersonaSummaryModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
}

public sealed class PermissionSummaryModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Title { get; set; }
}
