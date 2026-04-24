namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.SearchCategories;

using OysterFx.AppCore.Shared.Queries;

public class SearchCategoriesQuery : IQuery<SearchCategoriesQueryResult>
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public Guid? ParentCategoryRef { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
