namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands.CreatePriceChannel;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands.UpdatePriceChannel;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetActivePriceChannels;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetPriceChannelLookup;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.SearchPriceChannels;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class PriceChannelController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreatePriceChannelCommand command)
        => SendCommand<CreatePriceChannelCommand, CreatePriceChannelCommandResult>(command);

    [HttpPut("{priceChannelBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid priceChannelBusinessKey, [FromBody] UpdatePriceChannelCommand command)
    {
        command.PriceChannelBusinessKey = priceChannelBusinessKey;
        return SendCommand<UpdatePriceChannelCommand, UpdatePriceChannelCommandResult>(command);
    }

    [HttpGet("{priceChannelBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid priceChannelBusinessKey)
        => ExecuteQueryAsync<GetPriceChannelByBusinessKeyQuery, GetPriceChannelByBusinessKeyQueryResult>(
            new GetPriceChannelByBusinessKeyQuery(priceChannelBusinessKey));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchPriceChannelsQuery query)
        => ExecuteQueryAsync<SearchPriceChannelsQuery, SearchPriceChannelsQueryResult>(query);

    [HttpGet("active")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActivePriceChannelsQuery, GetActivePriceChannelsQueryResult>(new GetActivePriceChannelsQuery());

    [HttpGet("lookup")]
    public Task<IActionResult> GetLookup([FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetPriceChannelLookupQuery, GetPriceChannelLookupQueryResult>(
            new GetPriceChannelLookupQuery { IncludeInactive = includeInactive });
}
