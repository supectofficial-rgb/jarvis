namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.ActivateAttributeDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.ActivateAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.AddAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.CreateAttributeDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.CreateAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeactivateAttributeDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeactivateAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeleteAttributeDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeleteAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.RemoveAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.UpdateAttributeDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.UpdateAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetActiveAttributeDefinitions;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetActiveAttributeOptionsByAttributeId;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetAttributeOptionById;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetAttributeOptionsByAttributeId;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByScope;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetList;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetSummary;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeDefinitions;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeOptions;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class AttributeDefinitionController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateAttributeDefinitionCommand command)
        => SendCommand<CreateAttributeDefinitionCommand, CreateAttributeDefinitionCommandResult>(command);

    [HttpPut("{attributeDefinitionBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid attributeDefinitionBusinessKey, [FromBody] UpdateAttributeDefinitionCommand command)
    {
        command.AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        return SendCommand<UpdateAttributeDefinitionCommand, UpdateAttributeDefinitionCommandResult>(command);
    }

    [HttpDelete("{attributeDefinitionBusinessKey:guid}")]
    public Task<IActionResult> Delete([FromRoute] Guid attributeDefinitionBusinessKey)
        => SendCommand<DeleteAttributeDefinitionCommand, DeleteAttributeDefinitionCommandResult>(
            new DeleteAttributeDefinitionCommand { AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey });

    [HttpPost("{attributeDefinitionBusinessKey:guid}/activate")]
    public Task<IActionResult> Activate([FromRoute] Guid attributeDefinitionBusinessKey)
        => SendCommand<ActivateAttributeDefinitionCommand, ActivateAttributeDefinitionCommandResult>(
            new ActivateAttributeDefinitionCommand { AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey });

    [HttpPost("{attributeDefinitionBusinessKey:guid}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid attributeDefinitionBusinessKey)
        => SendCommand<DeactivateAttributeDefinitionCommand, DeactivateAttributeDefinitionCommandResult>(
            new DeactivateAttributeDefinitionCommand { AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey });

    [HttpPost("{attributeDefinitionBusinessKey:guid}/options")]
    public Task<IActionResult> CreateOption([FromRoute] Guid attributeDefinitionBusinessKey, [FromBody] CreateAttributeOptionCommand command)
    {
        command.AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        return SendCommand<CreateAttributeOptionCommand, CreateAttributeOptionCommandResult>(command);
    }

    [HttpPost("{attributeDefinitionBusinessKey:guid}/options/add")]
    public Task<IActionResult> AddOption([FromRoute] Guid attributeDefinitionBusinessKey, [FromBody] AddAttributeOptionCommand command)
    {
        command.AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        return SendCommand<AddAttributeOptionCommand, AddAttributeOptionCommandResult>(command);
    }

    [HttpPut("{attributeDefinitionBusinessKey:guid}/options/{optionBusinessKey:guid}")]
    public Task<IActionResult> UpdateOption(
        [FromRoute] Guid attributeDefinitionBusinessKey,
        [FromRoute] Guid optionBusinessKey,
        [FromBody] UpdateAttributeOptionCommand command)
    {
        command.AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey;
        command.OptionBusinessKey = optionBusinessKey;
        return SendCommand<UpdateAttributeOptionCommand, UpdateAttributeOptionCommandResult>(command);
    }

    [HttpDelete("{attributeDefinitionBusinessKey:guid}/options/{optionBusinessKey:guid}")]
    public Task<IActionResult> DeleteOption([FromRoute] Guid attributeDefinitionBusinessKey, [FromRoute] Guid optionBusinessKey)
        => SendCommand<DeleteAttributeOptionCommand, DeleteAttributeOptionCommandResult>(
            new DeleteAttributeOptionCommand
            {
                AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey,
                OptionBusinessKey = optionBusinessKey
            });

    [HttpDelete("{attributeDefinitionBusinessKey:guid}/options/by-value")]
    public Task<IActionResult> RemoveOption([FromRoute] Guid attributeDefinitionBusinessKey, [FromQuery] string value)
        => SendCommand<RemoveAttributeOptionCommand, RemoveAttributeOptionCommandResult>(
            new RemoveAttributeOptionCommand
            {
                AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey,
                Value = value
            });

    [HttpPost("{attributeDefinitionBusinessKey:guid}/options/{optionBusinessKey:guid}/activate")]
    public Task<IActionResult> ActivateOption([FromRoute] Guid attributeDefinitionBusinessKey, [FromRoute] Guid optionBusinessKey)
        => SendCommand<ActivateAttributeOptionCommand, ActivateAttributeOptionCommandResult>(
            new ActivateAttributeOptionCommand
            {
                AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey,
                OptionBusinessKey = optionBusinessKey
            });

    [HttpPost("{attributeDefinitionBusinessKey:guid}/options/{optionBusinessKey:guid}/deactivate")]
    public Task<IActionResult> DeactivateOption([FromRoute] Guid attributeDefinitionBusinessKey, [FromRoute] Guid optionBusinessKey)
        => SendCommand<DeactivateAttributeOptionCommand, DeactivateAttributeOptionCommandResult>(
            new DeactivateAttributeOptionCommand
            {
                AttributeDefinitionBusinessKey = attributeDefinitionBusinessKey,
                OptionBusinessKey = optionBusinessKey
            });

    [HttpGet("{attributeDefinitionBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid attributeDefinitionBusinessKey)
        => ExecuteQueryAsync<GetAttributeDefinitionByBusinessKeyQuery, GetAttributeDefinitionByBusinessKeyQueryResult>(
            new GetAttributeDefinitionByBusinessKeyQuery(attributeDefinitionBusinessKey));

    [HttpGet("by-id/{attributeDefinitionId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid attributeDefinitionId)
        => ExecuteQueryAsync<GetAttributeDefinitionByIdQuery, GetAttributeDefinitionByIdQueryResult>(
            new GetAttributeDefinitionByIdQuery(attributeDefinitionId));

    [HttpGet("list")]
    public Task<IActionResult> GetList([FromQuery] GetAttributeDefinitionListQuery query)
        => ExecuteQueryAsync<GetAttributeDefinitionListQuery, GetAttributeDefinitionListQueryResult>(query);

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchAttributeDefinitionsQuery query)
        => ExecuteQueryAsync<SearchAttributeDefinitionsQuery, SearchAttributeDefinitionsQueryResult>(query);

    [HttpGet("active")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActiveAttributeDefinitionsQuery, GetActiveAttributeDefinitionsQueryResult>(
            new GetActiveAttributeDefinitionsQuery());

    [HttpGet("by-scope/{scope}")]
    public Task<IActionResult> GetByScope([FromRoute] string scope, [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetAttributeDefinitionsByScopeQuery, GetAttributeDefinitionsByScopeQueryResult>(
            new GetAttributeDefinitionsByScopeQuery(scope, includeInactive));

    [HttpGet("{attributeDefinitionId:guid}/summary")]
    public Task<IActionResult> GetSummary([FromRoute] Guid attributeDefinitionId)
        => ExecuteQueryAsync<GetAttributeDefinitionSummaryQuery, GetAttributeDefinitionSummaryQueryResult>(
            new GetAttributeDefinitionSummaryQuery(attributeDefinitionId));

    [HttpGet("options/{optionId:guid}")]
    public Task<IActionResult> GetOptionById([FromRoute] Guid optionId)
        => ExecuteQueryAsync<GetAttributeOptionByIdQuery, GetAttributeOptionByIdQueryResult>(
            new GetAttributeOptionByIdQuery(optionId));

    [HttpGet("{attributeDefinitionId:guid}/options")]
    public Task<IActionResult> GetOptionsByAttributeId([FromRoute] Guid attributeDefinitionId)
        => ExecuteQueryAsync<GetAttributeOptionsByAttributeIdQuery, GetAttributeOptionsByAttributeIdQueryResult>(
            new GetAttributeOptionsByAttributeIdQuery(attributeDefinitionId));

    [HttpGet("{attributeDefinitionId:guid}/options/active")]
    public Task<IActionResult> GetActiveOptionsByAttributeId([FromRoute] Guid attributeDefinitionId)
        => ExecuteQueryAsync<GetActiveAttributeOptionsByAttributeIdQuery, GetActiveAttributeOptionsByAttributeIdQueryResult>(
            new GetActiveAttributeOptionsByAttributeIdQuery(attributeDefinitionId));

    [HttpGet("options/search")]
    public Task<IActionResult> SearchOptions([FromQuery] SearchAttributeOptionsQuery query)
        => ExecuteQueryAsync<SearchAttributeOptionsQuery, SearchAttributeOptionsQueryResult>(query);
}
