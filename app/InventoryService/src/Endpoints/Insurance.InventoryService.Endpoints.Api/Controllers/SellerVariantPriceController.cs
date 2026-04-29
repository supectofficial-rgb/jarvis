namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands.CreateSellerVariantPrice;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands.UpdateSellerVariantPrice;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.SearchSellerVariantPrices;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class SellerVariantPriceController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateSellerVariantPriceCommand command)
        => SendCommand<CreateSellerVariantPriceCommand, CreateSellerVariantPriceCommandResult>(command);

    [HttpPut("{sellerVariantPriceBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid sellerVariantPriceBusinessKey, [FromBody] UpdateSellerVariantPriceCommand command)
    {
        command.SellerVariantPriceBusinessKey = sellerVariantPriceBusinessKey;
        return SendCommand<UpdateSellerVariantPriceCommand, UpdateSellerVariantPriceCommandResult>(command);
    }

    [HttpGet("{sellerVariantPriceBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid sellerVariantPriceBusinessKey)
        => ExecuteQueryAsync<GetSellerVariantPriceByBusinessKeyQuery, GetSellerVariantPriceByBusinessKeyQueryResult>(
            new GetSellerVariantPriceByBusinessKeyQuery(sellerVariantPriceBusinessKey));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchSellerVariantPricesQuery query)
        => ExecuteQueryAsync<SearchSellerVariantPricesQuery, SearchSellerVariantPricesQueryResult>(query);
}
