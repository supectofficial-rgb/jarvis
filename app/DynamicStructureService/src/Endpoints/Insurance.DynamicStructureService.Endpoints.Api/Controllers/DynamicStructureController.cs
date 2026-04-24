namespace Insurance.DynamicStructureService.Endpoints.Api.Controllers;

using Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands.AddFormItem;
using Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands.CreateForm;
using Insurance.DynamicStructureService.AppCore.Shared.FormTypes.Commands.CreateFormType;
using Insurance.DynamicStructureService.AppCore.Shared.Forms.Queries.GetFormByBusinessKey;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/DynamicStructureService/[controller]")]
public class DynamicStructureController : OysterFxController
{
    [HttpPost("form-types")]
    public Task<IActionResult> CreateFormType([FromBody] CreateFormTypeCommand command)
        => SendCommand<CreateFormTypeCommand, CreateFormTypeCommandResult>(command);

    [HttpPost("forms")]
    public Task<IActionResult> CreateForm([FromBody] CreateFormCommand command)
        => SendCommand<CreateFormCommand, CreateFormCommandResult>(command);

    [HttpPost("forms/{formBusinessKey:guid}/items")]
    public Task<IActionResult> AddFormItem([FromRoute] Guid formBusinessKey, [FromBody] AddFormItemCommand command)
    {
        command.FormBusinessKey = formBusinessKey;
        return SendCommand<AddFormItemCommand, AddFormItemCommandResult>(command);
    }

    [HttpGet("forms/{formBusinessKey:guid}")]
    public Task<IActionResult> GetFormByBusinessKey([FromRoute] Guid formBusinessKey)
        => ExecuteQueryAsync<GetFormByBusinessKeyQuery, GetFormByBusinessKeyQueryResult>(
            new GetFormByBusinessKeyQuery(formBusinessKey));
}

