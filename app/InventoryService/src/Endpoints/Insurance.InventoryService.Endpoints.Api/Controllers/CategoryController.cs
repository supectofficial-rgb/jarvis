namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.ActivateCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.ActivateCategoryAttributeRule;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.AddCategoryAttributeRule;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.CreateCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeactivateCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeactivateCategoryAttributeRule;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeleteCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.MoveCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.RemoveCategoryAttributeRule;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.UpdateCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.UpdateCategoryAttributeRule;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetActiveCategories;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetActiveCategoryAttributeRulesByCategoryId;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetAttributes;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeFormDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeRuleById;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeRulesByCategoryId;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryBreadcrumb;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryCatalogSetup;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategorySummary;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryTree;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetChildCategories;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetProductLevelCategoryAttributeRulesByCategoryId;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetRequiredCategoryAttributeRulesByCategoryId;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetRootCategories;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetVariantLevelCategoryAttributeRulesByCategoryId;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.SearchCategories;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class CategoryController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        => SendCommand<CreateCategoryCommand, CreateCategoryCommandResult>(command);

    [HttpPut("{categoryBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid categoryBusinessKey, [FromBody] UpdateCategoryCommand command)
    {
        command.CategoryBusinessKey = categoryBusinessKey;
        return SendCommand<UpdateCategoryCommand, UpdateCategoryCommandResult>(command);
    }

    [HttpPost("{categoryBusinessKey:guid}/move")]
    public Task<IActionResult> Move([FromRoute] Guid categoryBusinessKey, [FromBody] MoveCategoryCommand command)
    {
        command.CategoryBusinessKey = categoryBusinessKey;
        return SendCommand<MoveCategoryCommand, MoveCategoryCommandResult>(command);
    }

    [HttpPost("{categoryBusinessKey:guid}/activate")]
    public Task<IActionResult> Activate([FromRoute] Guid categoryBusinessKey)
        => SendCommand<ActivateCategoryCommand, ActivateCategoryCommandResult>(
            new ActivateCategoryCommand { CategoryBusinessKey = categoryBusinessKey });

    [HttpPost("{categoryBusinessKey:guid}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid categoryBusinessKey)
        => SendCommand<DeactivateCategoryCommand, DeactivateCategoryCommandResult>(
            new DeactivateCategoryCommand { CategoryBusinessKey = categoryBusinessKey });

    [HttpDelete("{categoryBusinessKey:guid}")]
    public Task<IActionResult> Delete([FromRoute] Guid categoryBusinessKey)
        => SendCommand<DeleteCategoryCommand, DeleteCategoryCommandResult>(
            new DeleteCategoryCommand { CategoryBusinessKey = categoryBusinessKey });

    [HttpPost("{categoryBusinessKey:guid}/attribute-rules")]
    public Task<IActionResult> AddAttributeRule([FromRoute] Guid categoryBusinessKey, [FromBody] AddCategoryAttributeRuleCommand command)
    {
        command.CategoryBusinessKey = categoryBusinessKey;
        return SendCommand<AddCategoryAttributeRuleCommand, AddCategoryAttributeRuleCommandResult>(command);
    }

    [HttpPut("{categoryBusinessKey:guid}/attribute-rules/{attributeRef:guid}")]
    public Task<IActionResult> UpdateAttributeRule(
        [FromRoute] Guid categoryBusinessKey,
        [FromRoute] Guid attributeRef,
        [FromBody] UpdateCategoryAttributeRuleCommand command)
    {
        command.CategoryBusinessKey = categoryBusinessKey;
        command.AttributeRef = attributeRef;
        return SendCommand<UpdateCategoryAttributeRuleCommand, UpdateCategoryAttributeRuleCommandResult>(command);
    }

    [HttpPost("{categoryBusinessKey:guid}/attribute-rules/{attributeRef:guid}/activate")]
    public Task<IActionResult> ActivateAttributeRule([FromRoute] Guid categoryBusinessKey, [FromRoute] Guid attributeRef)
        => SendCommand<ActivateCategoryAttributeRuleCommand, ActivateCategoryAttributeRuleCommandResult>(
            new ActivateCategoryAttributeRuleCommand
            {
                CategoryBusinessKey = categoryBusinessKey,
                AttributeRef = attributeRef
            });

    [HttpPost("{categoryBusinessKey:guid}/attribute-rules/{attributeRef:guid}/deactivate")]
    public Task<IActionResult> DeactivateAttributeRule([FromRoute] Guid categoryBusinessKey, [FromRoute] Guid attributeRef)
        => SendCommand<DeactivateCategoryAttributeRuleCommand, DeactivateCategoryAttributeRuleCommandResult>(
            new DeactivateCategoryAttributeRuleCommand
            {
                CategoryBusinessKey = categoryBusinessKey,
                AttributeRef = attributeRef
            });

    [HttpDelete("{categoryBusinessKey:guid}/attribute-rules/{attributeRef:guid}")]
    public Task<IActionResult> RemoveAttributeRule([FromRoute] Guid categoryBusinessKey, [FromRoute] Guid attributeRef)
        => SendCommand<RemoveCategoryAttributeRuleCommand, RemoveCategoryAttributeRuleCommandResult>(
            new RemoveCategoryAttributeRuleCommand
            {
                CategoryBusinessKey = categoryBusinessKey,
                AttributeRef = attributeRef
            });

    [HttpGet("{categoryBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid categoryBusinessKey)
        => ExecuteQueryAsync<GetCategoryByBusinessKeyQuery, GetCategoryByBusinessKeyQueryResult>(
            new GetCategoryByBusinessKeyQuery(categoryBusinessKey));

    [HttpGet("by-id/{categoryId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid categoryId)
        => ExecuteQueryAsync<GetCategoryByIdQuery, GetCategoryByBusinessKeyQueryResult>(
            new GetCategoryByIdQuery(categoryId));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchCategoriesQuery query)
        => ExecuteQueryAsync<SearchCategoriesQuery, SearchCategoriesQueryResult>(query);

    [HttpGet("tree")]
    public Task<IActionResult> GetTree([FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetCategoryTreeQuery, GetCategoryTreeQueryResult>(
            new GetCategoryTreeQuery(includeInactive));

    [HttpGet("roots")]
    public Task<IActionResult> GetRootCategories([FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetRootCategoriesQuery, GetRootCategoriesQueryResult>(
            new GetRootCategoriesQuery(includeInactive));

    [HttpGet("{parentCategoryId:guid}/children")]
    public Task<IActionResult> GetChildCategories([FromRoute] Guid parentCategoryId, [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetChildCategoriesQuery, GetChildCategoriesQueryResult>(
            new GetChildCategoriesQuery(parentCategoryId, includeInactive));

    [HttpGet("active")]
    public Task<IActionResult> GetActiveCategories()
        => ExecuteQueryAsync<GetActiveCategoriesQuery, GetActiveCategoriesQueryResult>(new GetActiveCategoriesQuery());

    [HttpGet("{categoryId:guid}/summary")]
    public Task<IActionResult> GetSummary([FromRoute] Guid categoryId)
        => ExecuteQueryAsync<GetCategorySummaryQuery, GetCategorySummaryQueryResult>(
            new GetCategorySummaryQuery(categoryId));

    [HttpGet("{categoryId:guid}/breadcrumb")]
    public Task<IActionResult> GetBreadcrumb([FromRoute] Guid categoryId)
        => ExecuteQueryAsync<GetCategoryBreadcrumbQuery, GetCategoryBreadcrumbQueryResult>(
            new GetCategoryBreadcrumbQuery(categoryId));

    [HttpGet("{categoryBusinessKey:guid}/attributes")]
    public Task<IActionResult> GetAttributes(
        [FromRoute] Guid categoryBusinessKey,
        [FromQuery] bool includeInherited = true,
        [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetCategoryAttributesQuery, GetCategoryAttributesQueryResult>(
            new GetCategoryAttributesQuery(categoryBusinessKey, includeInherited, includeInactive));

    [HttpGet("attribute-rules/{categoryAttributeRuleId:guid}")]
    public Task<IActionResult> GetCategoryAttributeRuleById([FromRoute] Guid categoryAttributeRuleId)
        => ExecuteQueryAsync<GetCategoryAttributeRuleByIdQuery, GetCategoryAttributeRuleByIdQueryResult>(
            new GetCategoryAttributeRuleByIdQuery(categoryAttributeRuleId));

    [HttpGet("{categoryId:guid}/attribute-rules")]
    public Task<IActionResult> GetCategoryAttributeRulesByCategoryId(
        [FromRoute] Guid categoryId,
        [FromQuery] bool includeInherited = true,
        [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetCategoryAttributeRulesByCategoryIdQuery, GetCategoryAttributeRulesByCategoryIdQueryResult>(
            new GetCategoryAttributeRulesByCategoryIdQuery(categoryId, includeInherited, includeInactive));

    [HttpGet("{categoryId:guid}/attribute-rules/active")]
    public Task<IActionResult> GetActiveCategoryAttributeRulesByCategoryId([FromRoute] Guid categoryId, [FromQuery] bool includeInherited = true)
        => ExecuteQueryAsync<GetActiveCategoryAttributeRulesByCategoryIdQuery, GetActiveCategoryAttributeRulesByCategoryIdQueryResult>(
            new GetActiveCategoryAttributeRulesByCategoryIdQuery(categoryId, includeInherited));

    [HttpGet("{categoryId:guid}/attribute-rules/required")]
    public Task<IActionResult> GetRequiredCategoryAttributeRulesByCategoryId([FromRoute] Guid categoryId, [FromQuery] bool includeInherited = true)
        => ExecuteQueryAsync<GetRequiredCategoryAttributeRulesByCategoryIdQuery, GetRequiredCategoryAttributeRulesByCategoryIdQueryResult>(
            new GetRequiredCategoryAttributeRulesByCategoryIdQuery(categoryId, includeInherited));

    [HttpGet("{categoryId:guid}/attribute-rules/variant-level")]
    public Task<IActionResult> GetVariantLevelCategoryAttributeRulesByCategoryId([FromRoute] Guid categoryId, [FromQuery] bool includeInherited = true)
        => ExecuteQueryAsync<GetVariantLevelCategoryAttributeRulesByCategoryIdQuery, GetVariantLevelCategoryAttributeRulesByCategoryIdQueryResult>(
            new GetVariantLevelCategoryAttributeRulesByCategoryIdQuery(categoryId, includeInherited));

    [HttpGet("{categoryId:guid}/attribute-rules/product-level")]
    public Task<IActionResult> GetProductLevelCategoryAttributeRulesByCategoryId([FromRoute] Guid categoryId, [FromQuery] bool includeInherited = true)
        => ExecuteQueryAsync<GetProductLevelCategoryAttributeRulesByCategoryIdQuery, GetProductLevelCategoryAttributeRulesByCategoryIdQueryResult>(
            new GetProductLevelCategoryAttributeRulesByCategoryIdQuery(categoryId, includeInherited));

    [HttpGet("{categoryId:guid}/attribute-form-definition")]
    public Task<IActionResult> GetCategoryAttributeFormDefinition(
        [FromRoute] Guid categoryId,
        [FromQuery] bool includeInherited = true,
        [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetCategoryAttributeFormDefinitionQuery, GetCategoryAttributeFormDefinitionQueryResult>(
            new GetCategoryAttributeFormDefinitionQuery(categoryId, includeInherited, includeInactive));

    [HttpGet("{categoryId:guid}/catalog-setup")]
    public Task<IActionResult> GetCategoryCatalogSetup(
        [FromRoute] Guid categoryId,
        [FromQuery] bool includeInherited = true,
        [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetCategoryCatalogSetupQuery, GetCategoryCatalogSetupQueryResult>(
            new GetCategoryCatalogSetupQuery(categoryId, includeInherited, includeInactive));
}
