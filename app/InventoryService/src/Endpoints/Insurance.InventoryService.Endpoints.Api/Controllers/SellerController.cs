namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.ActivateSeller;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.CreateSeller;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.DeactivateSeller;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.DeleteSeller;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.SetSellerAsSystemOwner;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.UnsetSellerAsSystemOwner;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.UpdateSeller;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetActiveSellers;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByCode;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetSellerLookup;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetSellerSummary;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.SearchSellers;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class SellerController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateSellerCommand command)
        => SendCommand<CreateSellerCommand, CreateSellerCommandResult>(command);

    [HttpPut("{sellerBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid sellerBusinessKey, [FromBody] UpdateSellerCommand command)
    {
        command.SellerBusinessKey = sellerBusinessKey;
        return SendCommand<UpdateSellerCommand, UpdateSellerCommandResult>(command);
    }

    [HttpPost("{sellerBusinessKey:guid}/activate")]
    public Task<IActionResult> Activate([FromRoute] Guid sellerBusinessKey)
        => SendCommand<ActivateSellerCommand, ActivateSellerCommandResult>(
            new ActivateSellerCommand { SellerBusinessKey = sellerBusinessKey });

    [HttpPost("{sellerBusinessKey:guid}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid sellerBusinessKey)
        => SendCommand<DeactivateSellerCommand, DeactivateSellerCommandResult>(
            new DeactivateSellerCommand { SellerBusinessKey = sellerBusinessKey });

    [HttpDelete("{sellerBusinessKey:guid}")]
    public Task<IActionResult> Delete([FromRoute] Guid sellerBusinessKey)
        => SendCommand<DeleteSellerCommand, DeleteSellerCommandResult>(
            new DeleteSellerCommand { SellerBusinessKey = sellerBusinessKey });

    [HttpPost("{sellerBusinessKey:guid}/system-owner")]
    public Task<IActionResult> SetSystemOwner([FromRoute] Guid sellerBusinessKey, [FromBody] SetSellerAsSystemOwnerCommand command)
    {
        command.SellerBusinessKey = sellerBusinessKey;
        return SendCommand<SetSellerAsSystemOwnerCommand, SetSellerAsSystemOwnerCommandResult>(command);
    }

    [HttpDelete("{sellerBusinessKey:guid}/system-owner")]
    public Task<IActionResult> UnsetSystemOwner([FromRoute] Guid sellerBusinessKey)
        => SendCommand<UnsetSellerAsSystemOwnerCommand, UnsetSellerAsSystemOwnerCommandResult>(
            new UnsetSellerAsSystemOwnerCommand
            {
                SellerBusinessKey = sellerBusinessKey
            });

    [HttpGet("{sellerBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid sellerBusinessKey)
        => ExecuteQueryAsync<GetSellerByBusinessKeyQuery, GetSellerByBusinessKeyQueryResult>(
            new GetSellerByBusinessKeyQuery(sellerBusinessKey));

    [HttpGet("by-id/{sellerId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid sellerId)
        => ExecuteQueryAsync<GetSellerByIdQuery, GetSellerByBusinessKeyQueryResult>(
            new GetSellerByIdQuery(sellerId));

    [HttpGet("by-code/{code}")]
    public Task<IActionResult> GetByCode([FromRoute] string code)
        => ExecuteQueryAsync<GetSellerByCodeQuery, GetSellerByBusinessKeyQueryResult>(
            new GetSellerByCodeQuery(code));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchSellersQuery query)
        => ExecuteQueryAsync<SearchSellersQuery, SearchSellersQueryResult>(query);

    [HttpGet("active")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActiveSellersQuery, GetActiveSellersQueryResult>(new GetActiveSellersQuery());

    [HttpGet("lookup")]
    public Task<IActionResult> GetLookup([FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetSellerLookupQuery, GetSellerLookupQueryResult>(
            new GetSellerLookupQuery { IncludeInactive = includeInactive });

    [HttpGet("{sellerBusinessKey:guid}/summary")]
    public Task<IActionResult> GetSummary([FromRoute] Guid sellerBusinessKey)
        => ExecuteQueryAsync<GetSellerSummaryQuery, GetSellerSummaryQueryResult>(
            new GetSellerSummaryQuery(sellerBusinessKey));
}
