namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ArchiveEmptyStockDetail;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.EnsureStockDetailBucket;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.RebuildStockDetail;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.ReconcileStockDetail;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetAvailableStockBuckets;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBucketKey;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByLotBatch;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByLocation;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByQualityStatus;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetBySeller;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByVariant;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByWarehouse;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetEmptyStockBuckets;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetLowStockBuckets;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetSellerStockSummary;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetStockAging;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetVariantStockSummary;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetWarehouseStockSummary;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.SearchStockDetails;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class StockDetailController : OysterFxController
{
    [HttpPost("ensure-bucket")]
    public Task<IActionResult> EnsureBucket([FromBody] EnsureStockDetailBucketCommand command)
        => SendCommand<EnsureStockDetailBucketCommand, EnsureStockDetailBucketCommandResult>(command);

    [HttpPost("{stockDetailBusinessKey:guid}/rebuild")]
    public Task<IActionResult> Rebuild([FromRoute] Guid stockDetailBusinessKey, [FromBody] RebuildStockDetailCommand command)
    {
        command.StockDetailBusinessKey = stockDetailBusinessKey;
        return SendCommand<RebuildStockDetailCommand, RebuildStockDetailCommandResult>(command);
    }

    [HttpPost("{stockDetailBusinessKey:guid}/reconcile")]
    public Task<IActionResult> Reconcile([FromRoute] Guid stockDetailBusinessKey, [FromBody] ReconcileStockDetailCommand command)
    {
        command.StockDetailBusinessKey = stockDetailBusinessKey;
        return SendCommand<ReconcileStockDetailCommand, ReconcileStockDetailCommandResult>(command);
    }

    [HttpDelete("{stockDetailBusinessKey:guid}/archive-empty")]
    public Task<IActionResult> ArchiveEmpty([FromRoute] Guid stockDetailBusinessKey)
        => SendCommand<ArchiveEmptyStockDetailCommand, ArchiveEmptyStockDetailCommandResult>(
            new ArchiveEmptyStockDetailCommand { StockDetailBusinessKey = stockDetailBusinessKey });

    [HttpGet("{stockDetailBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid stockDetailBusinessKey)
        => ExecuteQueryAsync<GetStockDetailByBusinessKeyQuery, GetStockDetailByBusinessKeyQueryResult>(
            new GetStockDetailByBusinessKeyQuery(stockDetailBusinessKey));

    [HttpGet("by-id/{stockDetailId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid stockDetailId)
        => ExecuteQueryAsync<GetStockDetailByIdQuery, GetStockDetailByIdQueryResult>(
            new GetStockDetailByIdQuery(stockDetailId));

    [HttpGet("by-bucket")]
    public Task<IActionResult> GetByBucket([FromQuery] GetStockDetailByBucketKeyQuery query)
        => ExecuteQueryAsync<GetStockDetailByBucketKeyQuery, GetStockDetailByBucketKeyQueryResult>(query);

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchStockDetailsQuery query)
        => ExecuteQueryAsync<SearchStockDetailsQuery, SearchStockDetailsQueryResult>(query);

    [HttpGet("by-variant/{variantRef:guid}")]
    public Task<IActionResult> GetByVariant([FromRoute] Guid variantRef)
        => ExecuteQueryAsync<GetStockDetailsByVariantQuery, GetStockDetailsByVariantQueryResult>(
            new GetStockDetailsByVariantQuery(variantRef));

    [HttpGet("by-seller/{sellerRef:guid}")]
    public Task<IActionResult> GetBySeller([FromRoute] Guid sellerRef)
        => ExecuteQueryAsync<GetStockDetailsBySellerQuery, GetStockDetailsBySellerQueryResult>(
            new GetStockDetailsBySellerQuery(sellerRef));

    [HttpGet("by-warehouse/{warehouseRef:guid}")]
    public Task<IActionResult> GetByWarehouse([FromRoute] Guid warehouseRef)
        => ExecuteQueryAsync<GetStockDetailsByWarehouseQuery, GetStockDetailsByWarehouseQueryResult>(
            new GetStockDetailsByWarehouseQuery(warehouseRef));

    [HttpGet("by-location/{locationRef:guid}")]
    public Task<IActionResult> GetByLocation([FromRoute] Guid locationRef)
        => ExecuteQueryAsync<GetStockDetailsByLocationQuery, GetStockDetailsByLocationQueryResult>(
            new GetStockDetailsByLocationQuery(locationRef));

    [HttpGet("by-quality-status/{qualityStatusRef:guid}")]
    public Task<IActionResult> GetByQualityStatus([FromRoute] Guid qualityStatusRef)
        => ExecuteQueryAsync<GetStockDetailsByQualityStatusQuery, GetStockDetailsByQualityStatusQueryResult>(
            new GetStockDetailsByQualityStatusQuery(qualityStatusRef));

    [HttpGet("by-lot-batch/{lotBatchNo}")]
    public Task<IActionResult> GetByLotBatch([FromRoute] string lotBatchNo)
        => ExecuteQueryAsync<GetStockDetailsByLotBatchQuery, GetStockDetailsByLotBatchQueryResult>(
            new GetStockDetailsByLotBatchQuery(lotBatchNo));

    [HttpGet("available-buckets")]
    public Task<IActionResult> GetAvailableBuckets([FromQuery] GetAvailableStockBucketsQuery query)
        => ExecuteQueryAsync<GetAvailableStockBucketsQuery, GetAvailableStockBucketsQueryResult>(query);

    [HttpGet("empty-buckets")]
    public Task<IActionResult> GetEmptyBuckets([FromQuery] GetEmptyStockBucketsQuery query)
        => ExecuteQueryAsync<GetEmptyStockBucketsQuery, GetEmptyStockBucketsQueryResult>(query);

    [HttpGet("stock-aging")]
    public Task<IActionResult> GetStockAging([FromQuery] GetStockAgingQuery query)
        => ExecuteQueryAsync<GetStockAgingQuery, GetStockAgingQueryResult>(query);

    [HttpGet("low-stock-buckets")]
    public Task<IActionResult> GetLowStockBuckets([FromQuery] GetLowStockBucketsQuery query)
        => ExecuteQueryAsync<GetLowStockBucketsQuery, GetLowStockBucketsQueryResult>(query);

    [HttpGet("summary/variant/{variantRef:guid}")]
    public Task<IActionResult> GetVariantSummary([FromRoute] Guid variantRef)
        => ExecuteQueryAsync<GetVariantStockSummaryQuery, GetVariantStockSummaryQueryResult>(
            new GetVariantStockSummaryQuery(variantRef));

    [HttpGet("summary/warehouse/{warehouseRef:guid}")]
    public Task<IActionResult> GetWarehouseSummary([FromRoute] Guid warehouseRef)
        => ExecuteQueryAsync<GetWarehouseStockSummaryQuery, GetWarehouseStockSummaryQueryResult>(
            new GetWarehouseStockSummaryQuery(warehouseRef));

    [HttpGet("summary/seller/{sellerRef:guid}")]
    public Task<IActionResult> GetSellerSummary([FromRoute] Guid sellerRef)
        => ExecuteQueryAsync<GetSellerStockSummaryQuery, GetSellerStockSummaryQueryResult>(
            new GetSellerStockSummaryQuery(sellerRef));
}
