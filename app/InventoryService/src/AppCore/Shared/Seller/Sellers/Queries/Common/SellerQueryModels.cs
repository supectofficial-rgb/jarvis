namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.Common;

public class SellerListItem
{
    public Guid SellerBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystemOwner { get; set; }
    public bool IsActive { get; set; }
}

public class SellerLookupItem
{
    public Guid SellerBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class SellerSummaryItem
{
    public Guid SellerBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystemOwner { get; set; }
    public bool IsActive { get; set; }
}
