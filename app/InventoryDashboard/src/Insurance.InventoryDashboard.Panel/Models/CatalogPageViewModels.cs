using System.ComponentModel.DataAnnotations;

namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class CategoryManagementPageViewModel
{
    public string UserName { get; set; } = "کاربر";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DashboardMenuModule> Modules { get; set; } = Array.Empty<DashboardMenuModule>();
    public DashboardMenuModule? ActiveModule { get; set; }
    public DashboardMenuItem? ActiveItem { get; set; }

    public bool IsCategoryCreateMode { get; set; }
    public string? SelectedCategoryId { get; set; }
    public IReadOnlyList<CategoryNodeModel> Categories { get; set; } = Array.Empty<CategoryNodeModel>();
    public IReadOnlyList<CategoryNodeModel> FlatCategories { get; set; } = Array.Empty<CategoryNodeModel>();
    public IReadOnlyList<CategoryNodeModel> FilteredFlatCategories { get; set; } = Array.Empty<CategoryNodeModel>();
    public IReadOnlyList<AttributeDefinitionModel> AllAttributes { get; set; } = Array.Empty<AttributeDefinitionModel>();
    public IReadOnlyList<AttributeDefinitionModel> CategoryAttributes { get; set; } = Array.Empty<AttributeDefinitionModel>();
    public IReadOnlyList<CategoryAttributeRuleModel> CategoryAttributeRules { get; set; } = Array.Empty<CategoryAttributeRuleModel>();
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
    public string? CategorySearchTerm { get; set; }
    public string? CategoryStatusFilter { get; set; }
    public string? CategorySort { get; set; }
    public int CategoryPage { get; set; } = 1;
    public int CategoryPageSize { get; set; } = 10;
    public int CategoryTotalCount { get; set; }
    public int CategoryTotalPages { get; set; } = 1;
    public IReadOnlyList<int> CategoryPageSizeOptions { get; set; } = new[] { 10, 25, 50 };
    public string? ErrorMessage { get; set; }

    public CategoryUpsertForm CategoryForm { get; set; } = new();
    public MoveCategoryForm MoveCategoryForm { get; set; } = new();
    public AttributeDefinitionForm AttributeForm { get; set; } = new();
    public AttributeDefinitionUpdateForm AttributeUpdateForm { get; set; } = new();
    public AttributeOptionForm OptionForm { get; set; } = new();
    public AttributeOptionUpdateForm OptionUpdateForm { get; set; } = new();
    public AssignAttributeForm AssignForm { get; set; } = new();
    public CategoryAttributeRuleForm RuleForm { get; set; } = new();
}

public sealed class ProductManagementPageViewModel
{
    public string UserName { get; set; } = "کاربر";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DashboardMenuModule> Modules { get; set; } = Array.Empty<DashboardMenuModule>();
    public DashboardMenuModule? ActiveModule { get; set; }
    public DashboardMenuItem? ActiveItem { get; set; }

    public bool IsProductCreateMode { get; set; }
    public string? SelectedCategoryId { get; set; }
    public string? SelectedProductId { get; set; }
    public IReadOnlyList<CategoryNodeModel> Categories { get; set; } = Array.Empty<CategoryNodeModel>();
    public IReadOnlyList<CategoryNodeModel> FlatCategories { get; set; } = Array.Empty<CategoryNodeModel>();

    public IReadOnlyList<ProductSummaryModel> Products { get; set; } = Array.Empty<ProductSummaryModel>();
    public ProductDetailsModel? SelectedProductDetails { get; set; }
    public IReadOnlyList<ProductVariantSummaryModel> ProductVariants { get; set; } = Array.Empty<ProductVariantSummaryModel>();
    public IReadOnlyList<ProductVariantDetailsModel> ProductVariantDetails { get; set; } = Array.Empty<ProductVariantDetailsModel>();
    public int ProductVariantTotalCount { get; set; }
    public IReadOnlyList<UnitOfMeasureLookupModel> UnitOfMeasures { get; set; } = Array.Empty<UnitOfMeasureLookupModel>();
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();

    public IReadOnlyList<CategoryAttributeGroupViewModel> CategoryAttributeGroups { get; set; } = Array.Empty<CategoryAttributeGroupViewModel>();
    public IReadOnlyList<EffectiveAttributeViewModel> EffectiveCategoryAttributes { get; set; } = Array.Empty<EffectiveAttributeViewModel>();
    public IReadOnlyList<EffectiveAttributeViewModel> MissingRequiredProductAttributes { get; set; } = Array.Empty<EffectiveAttributeViewModel>();

    public string? ProductSearchTerm { get; set; }
    public string? ProductCategoryFilterId { get; set; }
    public string? ProductStatusFilter { get; set; }
    public string? ProductSort { get; set; }
    public int ProductPage { get; set; } = 1;
    public int ProductPageSize { get; set; } = 10;
    public int ProductTotalCount { get; set; }
    public int ProductTotalPages { get; set; } = 1;
    public IReadOnlyList<int> ProductPageSizeOptions { get; set; } = new[] { 10, 25, 50 };

    public string? ErrorMessage { get; set; }

    public ProductUpsertForm ProductForm { get; set; } = new();
    public ProductAttributeValueForm ProductAttributeForm { get; set; } = new();
    public ProductCategoryChangeForm ProductCategoryChangeForm { get; set; } = new();
}

public sealed class VariantManagementPageViewModel
{
    public string UserName { get; set; } = "کاربر";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DashboardMenuModule> Modules { get; set; } = Array.Empty<DashboardMenuModule>();
    public DashboardMenuModule? ActiveModule { get; set; }
    public DashboardMenuItem? ActiveItem { get; set; }

    public bool IsVariantCreateMode { get; set; }
    public string? SelectedProductId { get; set; }
    public string? SelectedVariantId { get; set; }
    public string? SelectedCategoryId { get; set; }

    public IReadOnlyList<CategoryNodeModel> Categories { get; set; } = Array.Empty<CategoryNodeModel>();
    public IReadOnlyList<CategoryNodeModel> FlatCategories { get; set; } = Array.Empty<CategoryNodeModel>();
    public IReadOnlyList<ProductSummaryModel> Products { get; set; } = Array.Empty<ProductSummaryModel>();
    public IReadOnlyList<ProductVariantSummaryModel> Variants { get; set; } = Array.Empty<ProductVariantSummaryModel>();
    public ProductVariantDetailsModel? SelectedVariantDetails { get; set; }
    public IReadOnlyList<UnitOfMeasureLookupModel> UnitOfMeasures { get; set; } = Array.Empty<UnitOfMeasureLookupModel>();
    public IReadOnlyList<VariantUomConversionModel> VariantUomConversions { get; set; } = Array.Empty<VariantUomConversionModel>();
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();

    public IReadOnlyList<CategoryAttributeGroupViewModel> ProductAttributeGroups { get; set; } = Array.Empty<CategoryAttributeGroupViewModel>();
    public IReadOnlyList<EffectiveAttributeViewModel> EffectiveProductAttributes { get; set; } = Array.Empty<EffectiveAttributeViewModel>();
    public IReadOnlyList<EffectiveAttributeViewModel> MissingRequiredVariantAttributes { get; set; } = Array.Empty<EffectiveAttributeViewModel>();

    public string? VariantSearchTerm { get; set; }
    public string? VariantCategoryFilterId { get; set; }
    public string? VariantAttributeOptionFilterIds { get; set; }
    public string? VariantTrackingFilter { get; set; }
    public string? VariantStatusFilter { get; set; }
    public string? SelectedAttributeTypeFilter { get; set; }
    public string? VariantSort { get; set; }
    public int VariantPage { get; set; } = 1;
    public int VariantPageSize { get; set; } = 10;
    public int VariantTotalCount { get; set; }
    public int VariantTotalPages { get; set; } = 1;
    public IReadOnlyList<int> VariantPageSizeOptions { get; set; } = new[] { 10, 25, 50 };

    public string? ErrorMessage { get; set; }

    public VariantUpsertForm VariantForm { get; set; } = new();
    public VariantAttributeValueForm VariantAttributeForm { get; set; } = new();
    public VariantUomConversionForm VariantUomConversionForm { get; set; } = new();
}

public sealed class CategoryAttributeGroupViewModel
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public bool IsCurrentCategory { get; set; }
    public bool IsInherited => !IsCurrentCategory;
    public IReadOnlyList<AttributeDefinitionModel> Attributes { get; set; } = Array.Empty<AttributeDefinitionModel>();
}

public sealed class EffectiveAttributeViewModel
{
    public string AttributeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = "Both";
    public bool IsVariantLevel { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public IReadOnlyList<AttributeOptionModel> Options { get; set; } = Array.Empty<AttributeOptionModel>();
    public string SourceCategoryId { get; set; } = string.Empty;
    public string SourceCategoryName { get; set; } = string.Empty;
    public bool IsInherited { get; set; }
}

public sealed class CategoryUpsertForm
{
    public string? CategoryId { get; set; }

    [Required(ErrorMessage = "کد دسته‌بندی الزامی است.")]
    [StringLength(64, ErrorMessage = "کد دسته‌بندی نمی‌تواند بیشتر از ۶۴ کاراکتر باشد.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام دسته‌بندی الزامی است.")]
    [StringLength(120, ErrorMessage = "نام دسته‌بندی نمی‌تواند بیشتر از ۱۲۰ کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "ترتیب نمایش باید عدد معتبر باشد.")]
    public int DisplayOrder { get; set; }

    [StringLength(500, ErrorMessage = "توضیح نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.")]
    public string? Description { get; set; }

    public string? ParentCategoryId { get; set; }
}

public sealed class AttributeDefinitionForm
{
    [Required]
    public string CategoryId { get; set; } = string.Empty;

    [Required(ErrorMessage = "کد اتریبیوت الزامی است.")]
    [StringLength(64, ErrorMessage = "کد اتریبیوت نمی‌تواند بیشتر از ۶۴ کاراکتر باشد.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام اتریبیوت الزامی است.")]
    [StringLength(120, ErrorMessage = "نام اتریبیوت نمی‌تواند بیشتر از ۱۲۰ کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع داده اتریبیوت الزامی است.")]
    public string DataType { get; set; } = "Text";

    [Required(ErrorMessage = "Scope اتریبیوت الزامی است.")]
    public string Scope { get; set; } = "Both";

    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }

    [Range(0, 100000, ErrorMessage = "ترتیب نمایش باید عدد معتبر باشد.")]
    public int DisplayOrder { get; set; }
}

public sealed class AttributeOptionForm
{
    [Required]
    public string CategoryId { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب اتریبیوت الزامی است.")]
    public string AttributeId { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام Option الزامی است.")]
    [StringLength(120, ErrorMessage = "نام Option نمی‌تواند بیشتر از ۱۲۰ کاراکتر باشد.")]
    public string OptionName { get; set; } = string.Empty;

    [Required(ErrorMessage = "مقدار Option الزامی است.")]
    [StringLength(120, ErrorMessage = "مقدار Option نمی‌تواند بیشتر از ۱۲۰ کاراکتر باشد.")]
    public string OptionValue { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "ترتیب نمایش باید عدد معتبر باشد.")]
    public int DisplayOrder { get; set; }
}

public sealed class AssignAttributeForm
{
    [Required(ErrorMessage = "انتخاب دسته‌بندی الزامی است.")]
    public string CategoryId { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب اتریبیوت الزامی است.")]
    public string AttributeId { get; set; } = string.Empty;

    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }

    [Range(0, 100000, ErrorMessage = "ترتیب نمایش باید عدد معتبر باشد.")]
    public int DisplayOrder { get; set; }

    public bool IsOverridden { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class MoveCategoryForm
{
    [Required]
    public string CategoryId { get; set; } = string.Empty;

    public string? ParentCategoryId { get; set; }
}

public sealed class CategoryAttributeRuleForm
{
    [Required]
    public string CategoryId { get; set; } = string.Empty;

    [Required]
    public string AttributeId { get; set; } = string.Empty;

    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }

    [Range(0, 100000, ErrorMessage = "ترتیب نمایش باید عدد معتبر باشد.")]
    public int DisplayOrder { get; set; }

    public bool IsOverridden { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class AttributeDefinitionUpdateForm
{
    [Required]
    public string CategoryId { get; set; } = string.Empty;

    [Required]
    public string AttributeId { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string DataType { get; set; } = "Text";

    [Required]
    public string Scope { get; set; } = "Both";

    [Range(0, 100000, ErrorMessage = "ترتیب نمایش باید عدد معتبر باشد.")]
    public int DisplayOrder { get; set; }

    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }

    public bool IsActive { get; set; } = true;
}

public sealed class AttributeOptionUpdateForm
{
    [Required]
    public string CategoryId { get; set; } = string.Empty;

    [Required]
    public string AttributeId { get; set; } = string.Empty;

    [Required]
    public string OptionId { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string OptionName { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string OptionValue { get; set; } = string.Empty;

    [Range(0, 100000)]
    public int DisplayOrder { get; set; }
}

public sealed class ProductUpsertForm
{
    public string? ProductId { get; set; }

    [Required(ErrorMessage = "نام محصول الزامی است.")]
    [StringLength(160, ErrorMessage = "نام محصول نمی‌تواند بیشتر از ۱۶۰ کاراکتر باشد.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "شماره محصول نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    public string BaseSku { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب دسته‌بندی الزامی است.")]
    public string CategoryId { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب واحد پایه الزامی است.")]
    public string DefaultUomRef { get; set; } = string.Empty;

    public string? TaxCategoryRef { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(500, ErrorMessage = "توضیح محصول نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.")]
    public string? Description { get; set; }

    public string? AutoVariantPayload { get; set; }
}

public sealed class ProductAttributeValueForm
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string CategoryId { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب اتریبیوت الزامی است.")]
    public string AttributeId { get; set; } = string.Empty;

    [Required(ErrorMessage = "مقدار اتریبیوت الزامی است.")]
    [StringLength(250, ErrorMessage = "طول مقدار اتریبیوت نمی‌تواند بیشتر از ۲۵۰ کاراکتر باشد.")]
    public string Value { get; set; } = string.Empty;
}

public sealed class VariantUpsertForm
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    public string? VariantId { get; set; }

    [Required(ErrorMessage = "SKU الزامی است.")]
    [StringLength(100, ErrorMessage = "SKU نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    public string Sku { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Barcode نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    public string? Barcode { get; set; }

    [Required(ErrorMessage = "Base UOM الزامی است.")]
    public string BaseUomRef { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tracking Policy الزامی است.")]
    public string TrackingPolicy { get; set; } = "None";

    public bool IsActive { get; set; } = true;
}

public sealed class VariantAttributeValueForm
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب Variant الزامی است.")]
    public string VariantId { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب اتریبیوت الزامی است.")]
    public string AttributeId { get; set; } = string.Empty;

    [Required(ErrorMessage = "مقدار اتریبیوت الزامی است.")]
    [StringLength(250, ErrorMessage = "طول مقدار اتریبیوت نمی‌تواند بیشتر از ۲۵۰ کاراکتر باشد.")]
    public string Value { get; set; } = string.Empty;
}

public sealed class ProductCategoryChangeForm
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string CurrentCategoryId { get; set; } = string.Empty;

    [Required]
    public string NewCategoryId { get; set; } = string.Empty;
}

public sealed class VariantUomConversionForm
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string VariantId { get; set; } = string.Empty;

    [Required]
    public string FromUomRef { get; set; } = string.Empty;

    [Required]
    public string ToUomRef { get; set; } = string.Empty;

    [Range(0.000001, double.MaxValue, ErrorMessage = "Factor باید بزرگ‌تر از صفر باشد.")]
    public decimal Factor { get; set; } = 1m;

    [Required]
    public string RoundingMode { get; set; } = "None";

    public bool IsBasePath { get; set; }
}
