namespace Insurance.AppCore.Domain.Losts.Enums;

/// <summary>
/// نوع پرونده
/// </summary>
public enum LostType : byte
{
    Unknown = 0,
    Collision = 1,
    Theft = 2,
    Vandalism = 3,
    NaturalDisaster = 4,
    Fire = 5,
    GlassDamage = 6,
    ThirdPartyLiability = 7
}