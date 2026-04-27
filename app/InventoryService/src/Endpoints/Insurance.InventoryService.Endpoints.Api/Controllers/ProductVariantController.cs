namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ActivateVariant;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ChangeVariantBaseUom;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ChangeVariantTrackingPolicy;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.CreateProductVariant;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.DeactivateVariant;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.DeleteVariant;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.LockVariantInventoryMovement;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantAttributeValue;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantUomConversion;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.SetVariantAttributeValue;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpdateProductVariant;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantUomConversion;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetActiveVariants;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetAttributeValueById;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetAttributeValuesByVariantId;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetAttributeValuesWithDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBarcode;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByProductId;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetBySku;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetDetailsWithAttributes;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetDetailsWithProductContext;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetFullDetails;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetMissingRequiredAttributes;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetSummary;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantCatalogForm;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantCompletionStatus;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantEditorData;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantUomConversionByPath;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantUomConversionsByVariantId;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.SearchVariants;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class ProductVariantController : OysterFxController
{
    [HttpPost]
    [RequirePermission("Inventory.ProductVariant.Create", "Catalog.Variant.Create")]
    public Task<IActionResult> Create([FromBody] CreateProductVariantCommand command)
        => SendCommand<CreateProductVariantCommand, CreateProductVariantCommandResult>(command);

    [HttpPut("{productVariantBusinessKey:guid}")]
    [RequirePermission("Inventory.ProductVariant.Update", "Catalog.Variant.Update")]
    public Task<IActionResult> Update([FromRoute] Guid productVariantBusinessKey, [FromBody] UpdateProductVariantCommand command)
    {
        command.ProductVariantBusinessKey = productVariantBusinessKey;
        return SendCommand<UpdateProductVariantCommand, UpdateProductVariantCommandResult>(command);
    }

    [HttpPost("{productVariantBusinessKey:guid}/activate")]
    [RequirePermission("Inventory.ProductVariant.Update", "Catalog.Variant.Activate")]
    public Task<IActionResult> Activate([FromRoute] Guid productVariantBusinessKey)
        => SendCommand<ActivateVariantCommand, ActivateVariantCommandResult>(
            new ActivateVariantCommand { ProductVariantBusinessKey = productVariantBusinessKey });

    [HttpPost("{productVariantBusinessKey:guid}/deactivate")]
    [RequirePermission("Inventory.ProductVariant.Update", "Catalog.Variant.Deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid productVariantBusinessKey)
        => SendCommand<DeactivateVariantCommand, DeactivateVariantCommandResult>(
            new DeactivateVariantCommand { ProductVariantBusinessKey = productVariantBusinessKey });

    [HttpDelete("{productVariantBusinessKey:guid}")]
    [RequirePermission("Inventory.ProductVariant.Delete", "Catalog.Variant.Delete")]
    public Task<IActionResult> Delete([FromRoute] Guid productVariantBusinessKey)
        => SendCommand<DeleteVariantCommand, DeleteVariantCommandResult>(
            new DeleteVariantCommand { ProductVariantBusinessKey = productVariantBusinessKey });

    [HttpPost("{productVariantBusinessKey:guid}/tracking-policy")]
    [RequirePermission("Inventory.ProductVariant.ChangeTrackingPolicy", "Catalog.Variant.ChangeTrackingPolicy")]
    public Task<IActionResult> ChangeTrackingPolicy([FromRoute] Guid productVariantBusinessKey, [FromBody] ChangeVariantTrackingPolicyCommand command)
    {
        command.ProductVariantBusinessKey = productVariantBusinessKey;
        return SendCommand<ChangeVariantTrackingPolicyCommand, ChangeVariantTrackingPolicyCommandResult>(command);
    }

    [HttpPost("{productVariantBusinessKey:guid}/base-uom")]
    [RequirePermission("Inventory.ProductVariant.ChangeBaseUom", "Catalog.Variant.ChangeBaseUom")]
    public Task<IActionResult> ChangeBaseUom([FromRoute] Guid productVariantBusinessKey, [FromBody] ChangeVariantBaseUomCommand command)
    {
        command.ProductVariantBusinessKey = productVariantBusinessKey;
        return SendCommand<ChangeVariantBaseUomCommand, ChangeVariantBaseUomCommandResult>(command);
    }

    [HttpPost("{productVariantBusinessKey:guid}/lock-inventory-movement")]
    [RequirePermission("Inventory.ProductVariant.LockInventoryMovement", "Catalog.Variant.LockInventoryMovement")]
    public Task<IActionResult> LockInventoryMovement([FromRoute] Guid productVariantBusinessKey)
        => SendCommand<LockVariantInventoryMovementCommand, LockVariantInventoryMovementCommandResult>(
            new LockVariantInventoryMovementCommand { ProductVariantBusinessKey = productVariantBusinessKey });

    [HttpPut("{productVariantBusinessKey:guid}/uom-conversions")]
    [RequirePermission("Inventory.ProductVariantUomConversion.Upsert", "Catalog.VariantUomConversion.Upsert")]
    public Task<IActionResult> UpsertUomConversion([FromRoute] Guid productVariantBusinessKey, [FromBody] UpsertVariantUomConversionCommand command)
    {
        command.ProductVariantBusinessKey = productVariantBusinessKey;
        return SendCommand<UpsertVariantUomConversionCommand, UpsertVariantUomConversionCommandResult>(command);
    }

    [HttpDelete("{productVariantBusinessKey:guid}/uom-conversions")]
    [RequirePermission("Inventory.ProductVariantUomConversion.Remove", "Catalog.VariantUomConversion.Remove")]
    public Task<IActionResult> RemoveUomConversion([FromRoute] Guid productVariantBusinessKey, [FromQuery] Guid fromUomRef, [FromQuery] Guid toUomRef)
        => SendCommand<RemoveVariantUomConversionCommand, RemoveVariantUomConversionCommandResult>(
            new RemoveVariantUomConversionCommand
            {
                ProductVariantBusinessKey = productVariantBusinessKey,
                FromUomRef = fromUomRef,
                ToUomRef = toUomRef
            });

    [HttpPut("{productVariantBusinessKey:guid}/attributes/{attributeRef:guid}")]
    [RequirePermission("Inventory.VariantAttributeValue.Set", "Catalog.VariantAttributeValue.Set")]
    public Task<IActionResult> SetAttributeValue(
        [FromRoute] Guid productVariantBusinessKey,
        [FromRoute] Guid attributeRef,
        [FromBody] SetVariantAttributeValueCommand command)
    {
        command.ProductVariantBusinessKey = productVariantBusinessKey;
        command.AttributeRef = attributeRef;
        return SendCommand<SetVariantAttributeValueCommand, SetVariantAttributeValueCommandResult>(command);
    }

    [HttpDelete("{productVariantBusinessKey:guid}/attributes/{attributeRef:guid}")]
    [RequirePermission("Inventory.VariantAttributeValue.Remove", "Catalog.VariantAttributeValue.Remove")]
    public Task<IActionResult> RemoveAttributeValue([FromRoute] Guid productVariantBusinessKey, [FromRoute] Guid attributeRef)
        => SendCommand<RemoveVariantAttributeValueCommand, RemoveVariantAttributeValueCommandResult>(
            new RemoveVariantAttributeValueCommand
            {
                ProductVariantBusinessKey = productVariantBusinessKey,
                AttributeRef = attributeRef
            });

    [HttpGet("{productVariantBusinessKey:guid}")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid productVariantBusinessKey)
        => ExecuteQueryAsync<GetProductVariantByBusinessKeyQuery, GetProductVariantByBusinessKeyQueryResult>(
            new GetProductVariantByBusinessKeyQuery(productVariantBusinessKey));

    [HttpGet("by-id/{variantId:guid}")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetById([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantByIdQuery, GetVariantByIdQueryResult>(new GetVariantByIdQuery(variantId));

    [HttpGet("by-sku/{variantSku}")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetBySku([FromRoute] string variantSku)
        => ExecuteQueryAsync<GetVariantBySkuQuery, GetVariantBySkuQueryResult>(new GetVariantBySkuQuery(variantSku));

    [HttpGet("by-barcode/{barcode}")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetByBarcode([FromRoute] string barcode)
        => ExecuteQueryAsync<GetVariantByBarcodeQuery, GetVariantByBarcodeQueryResult>(new GetVariantByBarcodeQuery(barcode));

    [HttpGet("by-product/{productId:guid}")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetByProductId([FromRoute] Guid productId, [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetVariantsByProductIdQuery, GetVariantsByProductIdQueryResult>(
            new GetVariantsByProductIdQuery(productId, includeInactive));

    [HttpGet("search")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> Search([FromQuery] SearchVariantsQuery query)
        => ExecuteQueryAsync<SearchVariantsQuery, SearchVariantsQueryResult>(query);

    [HttpGet("active")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActiveVariantsQuery, GetActiveVariantsQueryResult>(new GetActiveVariantsQuery());

    [HttpGet("{variantId:guid}/summary")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetSummary([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantSummaryQuery, GetVariantSummaryQueryResult>(new GetVariantSummaryQuery(variantId));

    [HttpGet("{variantId:guid}/details/attributes")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetDetailsWithAttributes([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantDetailsWithAttributesQuery, GetVariantDetailsWithAttributesQueryResult>(
            new GetVariantDetailsWithAttributesQuery(variantId));

    [HttpGet("{variantId:guid}/details/product-context")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetDetailsWithProductContext([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantDetailsWithProductContextQuery, GetVariantDetailsWithProductContextQueryResult>(
            new GetVariantDetailsWithProductContextQuery(variantId));

    [HttpGet("{variantId:guid}/details/full")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetFullDetails([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantFullDetailsQuery, GetVariantFullDetailsQueryResult>(
            new GetVariantFullDetailsQuery(variantId));

    [HttpGet("{variantId:guid}/attributes")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetAttributeValuesByVariantId([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantAttributeValuesByVariantIdQuery, GetVariantAttributeValuesByVariantIdQueryResult>(
            new GetVariantAttributeValuesByVariantIdQuery(variantId));

    [HttpGet("attributes/{variantAttributeValueId:guid}")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetAttributeValueById([FromRoute] Guid variantAttributeValueId)
        => ExecuteQueryAsync<GetVariantAttributeValueByIdQuery, GetVariantAttributeValueByIdQueryResult>(
            new GetVariantAttributeValueByIdQuery(variantAttributeValueId));

    [HttpGet("{variantId:guid}/attributes/with-definition")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetAttributeValuesWithDefinition([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantAttributeValuesWithDefinitionQuery, GetVariantAttributeValuesWithDefinitionQueryResult>(
            new GetVariantAttributeValuesWithDefinitionQuery(variantId));

    [HttpGet("{variantId:guid}/attributes/missing-required")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetMissingRequiredAttributes([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetMissingRequiredVariantAttributesQuery, GetMissingRequiredVariantAttributesQueryResult>(
            new GetMissingRequiredVariantAttributesQuery(variantId));

    [HttpGet("{variantId:guid}/uom-conversions")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetUomConversionsByVariantId([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantUomConversionsByVariantIdQuery, GetVariantUomConversionsByVariantIdQueryResult>(
            new GetVariantUomConversionsByVariantIdQuery(variantId));

    [HttpGet("{variantId:guid}/uom-conversions/path")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetUomConversionByPath([FromRoute] Guid variantId, [FromQuery] Guid fromUomRef, [FromQuery] Guid toUomRef)
        => ExecuteQueryAsync<GetVariantUomConversionByPathQuery, GetVariantUomConversionByPathQueryResult>(
            new GetVariantUomConversionByPathQuery(variantId, fromUomRef, toUomRef));

    [HttpGet("{variantId:guid}/catalog-form")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetCatalogForm([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantCatalogFormQuery, GetVariantCatalogFormQueryResult>(
            new GetVariantCatalogFormQuery(variantId));

    [HttpGet("{variantId:guid}/completion-status")]
    [RequirePermission("Inventory.ProductVariant.Read", "Catalog.Variant.View")]
    public Task<IActionResult> GetCompletionStatus([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantCompletionStatusQuery, GetVariantCompletionStatusQueryResult>(
            new GetVariantCompletionStatusQuery(variantId));

    [HttpGet("{variantId:guid}/editor-data")]
    public Task<IActionResult> GetEditorData([FromRoute] Guid variantId)
        => ExecuteQueryAsync<GetVariantEditorDataQuery, GetVariantEditorDataQueryResult>(
            new GetVariantEditorDataQuery(variantId));
}
