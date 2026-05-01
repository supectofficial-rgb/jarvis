using System.ComponentModel.DataAnnotations;

namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class WarehouseManagementPageViewModel
{
    public string UserName { get; set; } = "کاربر";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DashboardMenuModule> Modules { get; set; } = Array.Empty<DashboardMenuModule>();
    public DashboardMenuModule? ActiveModule { get; set; }
    public DashboardMenuItem? ActiveItem { get; set; }

    public string? SelectedWarehouseId { get; set; }
    public string? SelectedLocationId { get; set; }
    public string? ErrorMessage { get; set; }

    public IReadOnlyList<WarehouseListItemModel> Warehouses { get; set; } = Array.Empty<WarehouseListItemModel>();
    public IReadOnlyList<WarehouseLookupItemModel> WarehouseLookup { get; set; } = Array.Empty<WarehouseLookupItemModel>();
    public IReadOnlyList<LocationListItemModel> Locations { get; set; } = Array.Empty<LocationListItemModel>();
    public WarehouseWithLocationsModel? SelectedWarehouseDetails { get; set; }

    public string? WarehouseCodeFilter { get; set; }
    public string? WarehouseNameFilter { get; set; }
    public string? WarehouseStatusFilter { get; set; }
    public int WarehousePage { get; set; } = 1;
    public int WarehousePageSize { get; set; } = 10;
    public int WarehouseTotalCount { get; set; }
    public int WarehouseTotalPages { get; set; } = 1;

    public string? LocationCodeFilter { get; set; }
    public string? LocationTypeFilter { get; set; }
    public string? LocationAisleFilter { get; set; }
    public string? LocationRackFilter { get; set; }
    public string? LocationShelfFilter { get; set; }
    public string? LocationBinFilter { get; set; }
    public string? LocationStatusFilter { get; set; }
    public int LocationPage { get; set; } = 1;
    public int LocationPageSize { get; set; } = 10;
    public int LocationTotalCount { get; set; }
    public int LocationTotalPages { get; set; } = 1;

    public IReadOnlyList<int> PageSizeOptions { get; set; } = new[] { 10, 25, 50 };

    public WarehouseForm WarehouseForm { get; set; } = new();
    public LocationForm LocationForm { get; set; } = new();
}

public sealed class WarehouseListItemModel
{
    public string WarehouseBusinessKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class WarehouseLookupItemModel
{
    public string WarehouseBusinessKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class WarehouseWithLocationsModel
{
    public string WarehouseBusinessKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<WarehouseLocationItemModel> Locations { get; set; } = new();
}

public sealed class WarehouseLocationItemModel
{
    public string LocationBusinessKey { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Shelf { get; set; }
    public string? Bin { get; set; }
    public bool IsActive { get; set; }
}

public sealed class LocationListItemModel
{
    public string LocationBusinessKey { get; set; } = string.Empty;
    public string WarehouseRef { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Shelf { get; set; }
    public string? Bin { get; set; }
    public bool IsActive { get; set; }
}

public sealed class WarehouseSearchResultModel
{
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<WarehouseListItemModel> Items { get; set; } = new();
}

public sealed class LocationSearchResultModel
{
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<LocationListItemModel> Items { get; set; } = new();
}

public sealed class WarehouseForm
{
    public string? WarehouseId { get; set; }

    [Required(ErrorMessage = "کد انبار الزامی است.")]
    [StringLength(50, ErrorMessage = "کد انبار نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام انبار الزامی است.")]
    [StringLength(160, ErrorMessage = "نام انبار نمی‌تواند بیشتر از ۱۶۰ کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public sealed class LocationForm
{
    public string? LocationId { get; set; }

    [Required(ErrorMessage = "انتخاب انبار الزامی است.")]
    public string WarehouseId { get; set; } = string.Empty;

    [Required(ErrorMessage = "کد لوکیشن الزامی است.")]
    [StringLength(80, ErrorMessage = "کد لوکیشن نمی‌تواند بیشتر از ۸۰ کاراکتر باشد.")]
    public string LocationCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع لوکیشن الزامی است.")]
    [StringLength(60, ErrorMessage = "نوع لوکیشن نمی‌تواند بیشتر از ۶۰ کاراکتر باشد.")]
    public string LocationType { get; set; } = "Bulk";

    [StringLength(40)]
    public string? Aisle { get; set; }

    [StringLength(40)]
    public string? Rack { get; set; }

    [StringLength(40)]
    public string? Shelf { get; set; }

    [StringLength(40)]
    public string? Bin { get; set; }

    public bool IsActive { get; set; } = true;
}

public sealed class UpsertWarehouseRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class UpsertLocationRequest
{
    public string WarehouseId { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Shelf { get; set; }
    public string? Bin { get; set; }
    public bool IsActive { get; set; } = true;
}
