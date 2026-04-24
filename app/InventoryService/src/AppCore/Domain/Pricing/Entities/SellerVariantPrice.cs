namespace Insurance.InventoryService.AppCore.Domain.Pricing.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;

public sealed class SellerVariantPrice : AggregateRoot
{
    private readonly List<Offer> _offers = new();

    public Guid SellerRef { get; private set; }
    public Guid VariantRef { get; private set; }
    public Guid PriceTypeRef { get; private set; }
    public Guid PriceChannelRef { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public decimal MinQty { get; private set; }
    public int Priority { get; private set; }
    public DateTime? EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<Offer> Offers => _offers.AsReadOnly();

    private SellerVariantPrice()
    {
    }

    private SellerVariantPrice(
        Guid sellerRef,
        Guid variantRef,
        Guid priceTypeRef,
        Guid priceChannelRef,
        decimal amount,
        string currency,
        decimal minQty,
        int priority,
        DateTime? effectiveFrom,
        DateTime? effectiveTo)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        if (minQty < 0)
            throw new ArgumentOutOfRangeException(nameof(minQty));
        if (effectiveFrom.HasValue && effectiveTo.HasValue && effectiveFrom > effectiveTo)
            throw new AggregateStateExceptions("Effective range is invalid.", nameof(effectiveTo));

        SellerRef = sellerRef;
        VariantRef = variantRef;
        PriceTypeRef = priceTypeRef;
        PriceChannelRef = priceChannelRef;
        Amount = amount;
        Currency = NormalizeRequired(currency, nameof(currency)).ToUpperInvariant();
        MinQty = minQty;
        Priority = priority;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        IsActive = true;
    }

    public static SellerVariantPrice Create(
        Guid sellerRef,
        Guid variantRef,
        Guid priceTypeRef,
        Guid priceChannelRef,
        decimal amount,
        string currency,
        decimal minQty = 0,
        int priority = 0,
        DateTime? effectiveFrom = null,
        DateTime? effectiveTo = null)
    {
        return new SellerVariantPrice(
            sellerRef,
            variantRef,
            priceTypeRef,
            priceChannelRef,
            amount,
            currency,
            minQty,
            priority,
            effectiveFrom,
            effectiveTo);
    }

    public void ChangeAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Amount = amount;
    }

    public void ChangeMinQty(decimal minQty)
    {
        if (minQty < 0)
            throw new ArgumentOutOfRangeException(nameof(minQty));

        MinQty = minQty;
    }

    public void ChangePriority(int priority) => Priority = priority;

    public void SetEffectiveWindow(DateTime? effectiveFrom, DateTime? effectiveTo)
    {
        if (effectiveFrom.HasValue && effectiveTo.HasValue && effectiveFrom > effectiveTo)
            throw new AggregateStateExceptions("Effective range is invalid.", nameof(effectiveTo));

        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public Offer AddOffer(
        string name,
        decimal? discountAmount,
        decimal? discountPercent,
        decimal? maxQuantity,
        int priority,
        DateTime? startAt,
        DateTime? endAt)
    {
        var offer = Offer.Create(BusinessKey.Value, name, discountAmount, discountPercent, maxQuantity, priority, startAt, endAt);
        _offers.Add(offer);
        return offer;
    }

    public void RemoveOffer(Guid offerBusinessKey)
    {
        var existing = _offers.FirstOrDefault(x => x.BusinessKey.Value == offerBusinessKey);
        if (existing is not null)
            _offers.Remove(existing);
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }
}
