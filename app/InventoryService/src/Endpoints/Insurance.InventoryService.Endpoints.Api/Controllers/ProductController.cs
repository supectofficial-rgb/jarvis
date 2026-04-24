namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.ActivateProduct;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.ChangeProductCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.CreateProduct;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.DeactivateProduct;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.DeleteProduct;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.RemoveProductAttributeValue;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.SetProductAttributeValue;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.UpdateProduct;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetActiveProducts;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValueById;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValuesByProductId;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValuesWithDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByCategoryId;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetDetailsWithAttributes;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetDetailsWithVariants;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetFullDetails;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetMissingRequiredAttributes;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetProductCatalogForm;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetProductCompletionStatus;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetProductEditorData;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetSummary;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.SearchProducts;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class ProductController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateProductCommand command)
        => SendCommand<CreateProductCommand, CreateProductCommandResult>(command);

    [HttpPut("{productBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid productBusinessKey, [FromBody] UpdateProductCommand command)
    {
        command.ProductBusinessKey = productBusinessKey;
        return SendCommand<UpdateProductCommand, UpdateProductCommandResult>(command);
    }

    [HttpPost("{productBusinessKey:guid}/activate")]
    public Task<IActionResult> Activate([FromRoute] Guid productBusinessKey)
        => SendCommand<ActivateProductCommand, ActivateProductCommandResult>(
            new ActivateProductCommand { ProductBusinessKey = productBusinessKey });

    [HttpPost("{productBusinessKey:guid}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid productBusinessKey)
        => SendCommand<DeactivateProductCommand, DeactivateProductCommandResult>(
            new DeactivateProductCommand { ProductBusinessKey = productBusinessKey });

    [HttpDelete("{productBusinessKey:guid}")]
    public Task<IActionResult> Delete([FromRoute] Guid productBusinessKey)
        => SendCommand<DeleteProductCommand, DeleteProductCommandResult>(
            new DeleteProductCommand { ProductBusinessKey = productBusinessKey });

    [HttpPost("{productBusinessKey:guid}/change-category")]
    public Task<IActionResult> ChangeCategory([FromRoute] Guid productBusinessKey, [FromBody] ChangeProductCategoryCommand command)
    {
        command.ProductBusinessKey = productBusinessKey;
        return SendCommand<ChangeProductCategoryCommand, ChangeProductCategoryCommandResult>(command);
    }

    [HttpPut("{productBusinessKey:guid}/attributes/{attributeRef:guid}")]
    public Task<IActionResult> SetAttributeValue(
        [FromRoute] Guid productBusinessKey,
        [FromRoute] Guid attributeRef,
        [FromBody] SetProductAttributeValueCommand command)
    {
        command.ProductBusinessKey = productBusinessKey;
        command.AttributeRef = attributeRef;
        return SendCommand<SetProductAttributeValueCommand, SetProductAttributeValueCommandResult>(command);
    }

    [HttpDelete("{productBusinessKey:guid}/attributes/{attributeRef:guid}")]
    public Task<IActionResult> RemoveAttributeValue([FromRoute] Guid productBusinessKey, [FromRoute] Guid attributeRef)
        => SendCommand<RemoveProductAttributeValueCommand, RemoveProductAttributeValueCommandResult>(
            new RemoveProductAttributeValueCommand
            {
                ProductBusinessKey = productBusinessKey,
                AttributeRef = attributeRef
            });

    [HttpGet("{productBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid productBusinessKey)
        => ExecuteQueryAsync<GetProductByBusinessKeyQuery, GetProductByBusinessKeyQueryResult>(
            new GetProductByBusinessKeyQuery(productBusinessKey));

    [HttpGet("by-id/{productId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductByIdQuery, GetProductByIdQueryResult>(
            new GetProductByIdQuery(productId));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchProductsQuery query)
        => ExecuteQueryAsync<SearchProductsQuery, SearchProductsQueryResult>(query);

    [HttpGet("by-category/{categoryId:guid}")]
    public Task<IActionResult> GetByCategoryId([FromRoute] Guid categoryId, [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetProductsByCategoryIdQuery, GetProductsByCategoryIdQueryResult>(
            new GetProductsByCategoryIdQuery(categoryId, includeInactive));

    [HttpGet("active")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActiveProductsQuery, GetActiveProductsQueryResult>(new GetActiveProductsQuery());

    [HttpGet("{productId:guid}/summary")]
    public Task<IActionResult> GetSummary([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductSummaryQuery, GetProductSummaryQueryResult>(
            new GetProductSummaryQuery(productId));

    [HttpGet("{productId:guid}/details/attributes")]
    public Task<IActionResult> GetDetailsWithAttributes([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductDetailsWithAttributesQuery, GetProductDetailsWithAttributesQueryResult>(
            new GetProductDetailsWithAttributesQuery(productId));

    [HttpGet("{productId:guid}/details/variants")]
    public Task<IActionResult> GetDetailsWithVariants([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductDetailsWithVariantsQuery, GetProductDetailsWithVariantsQueryResult>(
            new GetProductDetailsWithVariantsQuery(productId));

    [HttpGet("{productId:guid}/details/full")]
    public Task<IActionResult> GetFullDetails([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductFullDetailsQuery, GetProductFullDetailsQueryResult>(
            new GetProductFullDetailsQuery(productId));

    [HttpGet("{productId:guid}/attributes")]
    public Task<IActionResult> GetAttributeValuesByProductId([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductAttributeValuesByProductIdQuery, GetProductAttributeValuesByProductIdQueryResult>(
            new GetProductAttributeValuesByProductIdQuery(productId));

    [HttpGet("attributes/{productAttributeValueId:guid}")]
    public Task<IActionResult> GetAttributeValueById([FromRoute] Guid productAttributeValueId)
        => ExecuteQueryAsync<GetProductAttributeValueByIdQuery, GetProductAttributeValueByIdQueryResult>(
            new GetProductAttributeValueByIdQuery(productAttributeValueId));

    [HttpGet("{productId:guid}/attributes/with-definition")]
    public Task<IActionResult> GetAttributeValuesWithDefinition([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductAttributeValuesWithDefinitionQuery, GetProductAttributeValuesWithDefinitionQueryResult>(
            new GetProductAttributeValuesWithDefinitionQuery(productId));

    [HttpGet("{productId:guid}/attributes/missing-required")]
    public Task<IActionResult> GetMissingRequiredAttributes([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetMissingRequiredProductAttributesQuery, GetMissingRequiredProductAttributesQueryResult>(
            new GetMissingRequiredProductAttributesQuery(productId));

    [HttpGet("{productId:guid}/catalog-form")]
    public Task<IActionResult> GetCatalogForm([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductCatalogFormQuery, GetProductCatalogFormQueryResult>(
            new GetProductCatalogFormQuery(productId));

    [HttpGet("{productId:guid}/completion-status")]
    public Task<IActionResult> GetCompletionStatus([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductCompletionStatusQuery, GetProductCompletionStatusQueryResult>(
            new GetProductCompletionStatusQuery(productId));

    [HttpGet("{productId:guid}/editor-data")]
    public Task<IActionResult> GetEditorData([FromRoute] Guid productId)
        => ExecuteQueryAsync<GetProductEditorDataQuery, GetProductEditorDataQueryResult>(
            new GetProductEditorDataQuery(productId));
}
