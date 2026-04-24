namespace Insurance.AppCore.Domain.Losts.Dtos;

using Insurance.AppCore.Domain.Losts.Enums;

/// <summary>
/// اطلاعات ماک/راننده
/// </summary>
public class PolicyHolderInfo
{
    public PolicyHolderGender Gender { get; set; }
    public string? FullName { get; set; }
}