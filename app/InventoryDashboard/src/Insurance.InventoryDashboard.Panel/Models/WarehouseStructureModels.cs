using System.ComponentModel.DataAnnotations;

namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class WarehouseStructurePageViewModel
{
    public string UserName { get; set; } = "کاربر";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DashboardMenuModule> Modules { get; set; } = Array.Empty<DashboardMenuModule>();
    public DashboardMenuModule? ActiveModule { get; set; }
    public DashboardMenuItem? ActiveItem { get; set; }

    public IReadOnlyList<WarehouseLookupItemModel> WarehouseLookup { get; set; } = Array.Empty<WarehouseLookupItemModel>();
    public string? SelectedWarehouseId { get; set; }
    public string? ErrorMessage { get; set; }
    public IReadOnlyList<LocationStructureTreeItemModel> Structures { get; set; } = Array.Empty<LocationStructureTreeItemModel>();
    public string? SelectedStructureId { get; set; }
    public string StructureMode { get; set; } = "manage";
    public string StructureAjaxTarget { get; set; } = "#warehouse-structure-content";

    public LocationStructureNodeForm StructureNodeForm { get; set; } = new();
    public LocationStructureValueForm StructureValueForm { get; set; } = new();
}

public sealed class LocationStructureTreeItemModel
{
    public string LocationStructureBusinessKey { get; set; } = string.Empty;
    public string WarehouseRef { get; set; } = string.Empty;
    public string? ParentStructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<LocationStructureTreeItemModel> Children { get; set; } = new();
    public List<LocationStructureValueItemModel> Values { get; set; } = new();
}

public sealed class LocationStructureValueItemModel
{
    public string LocationStructureValueBusinessKey { get; set; } = string.Empty;
    public string StructureRef { get; set; } = string.Empty;

    public string WarehouseRef { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class LocationStructureValueCreateResultModel
{
    public string LocationStructureValueBusinessKey { get; set; } = string.Empty;
    public string StructureRef { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class LocationStructureNodeForm
{
    public string? LocationStructureBusinessKey { get; set; }

    [Required(ErrorMessage = "انتخاب انبار الزامی است.")]
    public string WarehouseRef { get; set; } = string.Empty;

    public string? ParentStructureRef { get; set; }

    [Required(ErrorMessage = "کد ساختار الزامی است.")]
    [StringLength(50, ErrorMessage = "کد ساختار نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام ساختار الزامی است.")]
    [StringLength(160, ErrorMessage = "نام ساختار نمی‌تواند بیشتر از ۱۶۰ کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class LocationStructureValueForm
{
    public string? LocationStructureValueBusinessKey { get; set; }

    [Required(ErrorMessage = "انتخاب ساختار الزامی است.")]
    public string StructureRef { get; set; } = string.Empty;

    [Required(ErrorMessage = "کد مقدار الزامی است.")]
    [StringLength(50, ErrorMessage = "کد مقدار نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام مقدار الزامی است.")]
    [StringLength(160, ErrorMessage = "نام مقدار نمی‌تواند بیشتر از ۱۶۰ کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
