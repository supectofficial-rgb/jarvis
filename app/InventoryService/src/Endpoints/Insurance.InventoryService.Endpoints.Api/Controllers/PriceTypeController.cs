namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands.CreatePriceType;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands.UpdatePriceType;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetActivePriceTypes;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetPriceTypeLookup;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.SearchPriceTypes;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class PriceTypeController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreatePriceTypeCommand command)
        => SendCommand<CreatePriceTypeCommand, CreatePriceTypeCommandResult>(command);

    [HttpPut("{priceTypeBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid priceTypeBusinessKey, [FromBody] UpdatePriceTypeCommand command)
    {
        command.PriceTypeBusinessKey = priceTypeBusinessKey;
        return SendCommand<UpdatePriceTypeCommand, UpdatePriceTypeCommandResult>(command);
    }

    [HttpGet("{priceTypeBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid priceTypeBusinessKey)
        => ExecuteQueryAsync<GetPriceTypeByBusinessKeyQuery, GetPriceTypeByBusinessKeyQueryResult>(
            new GetPriceTypeByBusinessKeyQuery(priceTypeBusinessKey));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchPriceTypesQuery query)
        => ExecuteQueryAsync<SearchPriceTypesQuery, SearchPriceTypesQueryResult>(query);

    [HttpGet("active")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActivePriceTypesQuery, GetActivePriceTypesQueryResult>(new GetActivePriceTypesQuery());

    [HttpGet("lookup")]
    public Task<IActionResult> GetLookup([FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetPriceTypeLookupQuery, GetPriceTypeLookupQueryResult>(
            new GetPriceTypeLookupQuery { IncludeInactive = includeInactive });
}
