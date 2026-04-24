namespace Insurance.GraphService.Endpoints.Api.Controllers;

using Insurance.GraphService.AppCore.Shared.GraphNodes.Commands.UpsertGraphNode;
using Insurance.GraphService.AppCore.Shared.GraphRelations.Commands.UpsertGraphRelation;
using Insurance.GraphService.AppCore.Shared.GraphNodes.Queries.GetNodesByType;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/GraphService/[controller]")]
public class GraphController : OysterFxController
{
    [HttpPost("nodes")]
    public Task<IActionResult> UpsertNode([FromBody] UpsertGraphNodeCommand command)
        => SendCommand<UpsertGraphNodeCommand, UpsertGraphNodeCommandResult>(command);

    [HttpPost("relations")]
    public Task<IActionResult> UpsertRelation([FromBody] UpsertGraphRelationCommand command)
        => SendCommand<UpsertGraphRelationCommand, UpsertGraphRelationCommandResult>(command);

    [HttpPost("nodes/by-type")]
    public Task<IActionResult> GetNodesByType([FromBody] GetNodesByTypeQuery query)
        => ExecuteQueryAsync<GetNodesByTypeQuery, GetNodesByTypeQueryResult>(query);
}

