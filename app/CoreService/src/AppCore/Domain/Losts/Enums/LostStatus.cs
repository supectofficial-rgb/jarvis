namespace Insurance.AppCore.Domain.Losts.Enums;

/// <summary>
/// وضعیت پرونده
/// </summary>
public enum LostStatus : byte
{
    Pending = 0,
    Assigned = 1,
    UnderInspection = 2,
    UnderReview = 3,
    Approved = 4,
    Rejected = 5,
    Paid = 6,
    Closed = 7
}