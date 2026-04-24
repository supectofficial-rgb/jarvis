namespace Insurance.InventoryService.AppCore.Domain.Pricing.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class Offer : Aggregate
{
    public Guid PriceRef { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal? DiscountAmount { get; private set; }
    public decimal? DiscountPercent { get; private set; }
    public decimal? MaxQuantity { get; private set; }
    public int Priority { get; private set; }
    public DateTime? StartAt { get; private set; }
    public DateTime? EndAt { get; private set; }
    public bool IsActive { get; private set; }

    private Offer()
    {
    }

    internal static Offer Create(
        Guid priceRef,
        string name,
        decimal? discountAmount,
        decimal? discountPercent,
        decimal? maxQuantity,
        int priority,
        DateTime? startAt,
        DateTime? endAt)
    {
        if (discountAmount.HasValue && discountAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(discountAmount));
        if (discountPercent.HasValue && (discountPercent < 0 || discountPercent > 100))
            throw new ArgumentOutOfRangeException(nameof(discountPercent));
        if (maxQuantity.HasValue && maxQuantity < 0)
            throw new ArgumentOutOfRangeException(nameof(maxQuantity));
        if (startAt.HasValue && endAt.HasValue && startAt > endAt)
            throw new InvalidOperationException("Offer window is invalid.");

        return new Offer
        {
            PriceRef = priceRef,
            Name = NormalizeRequired(name, nameof(name)),
            DiscountAmount = discountAmount,
            DiscountPercent = discountPercent,
            MaxQuantity = maxQuantity,
            Priority = priority,
            StartAt = startAt,
            EndAt = endAt,
            IsActive = true
        };
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void ChangeWindow(DateTime? startAt, DateTime? endAt)
    {
        if (startAt.HasValue && endAt.HasValue && startAt > endAt)
            throw new InvalidOperationException("Offer window is invalid.");

        StartAt = startAt;
        EndAt = endAt;
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }
}
