namespace Insurance.AppCore.Domain.Profiles.Enums;

/// <summary>
/// وضعیت نظام وظیفه
/// </summary>
public enum MilitaryServiceStatus : byte
{
    Unknown = 0,
    ExemptMedical = 1,
    ExemptGuarantee = 2,
    ExemptEducational = 3,
    ServiceCompleted = 4,
    Other = 5
}
