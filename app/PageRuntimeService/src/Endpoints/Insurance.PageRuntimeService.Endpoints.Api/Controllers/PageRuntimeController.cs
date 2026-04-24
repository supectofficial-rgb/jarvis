namespace Insurance.PageRuntimeService.Endpoints.Api.Controllers;

using Insurance.PageRuntimeService.AppCore.Shared.Languages.Commands.CreateLanguage;
using Insurance.PageRuntimeService.AppCore.Shared.Sections.Commands.CreateSection;
using Insurance.PageRuntimeService.AppCore.Shared.Sections.Queries.GetSectionByUrl;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/PageRuntimeService/[controller]")]
public class PageRuntimeController : OysterFxController
{
    [HttpPost("languages")]
    public Task<IActionResult> CreateLanguage([FromBody] CreateLanguageCommand command)
        => SendCommand<CreateLanguageCommand, CreateLanguageCommandResult>(command);

    [HttpPost("sections")]
    public Task<IActionResult> CreateSection([FromBody] CreateSectionCommand command)
        => SendCommand<CreateSectionCommand, CreateSectionCommandResult>(command);

    [HttpGet("sections/by-url")]
    public Task<IActionResult> GetSectionByUrl([FromQuery] string url, [FromQuery] string? lang)
        => ExecuteQueryAsync<GetSectionByUrlQuery, GetSectionByUrlQueryResult>(
            new GetSectionByUrlQuery { Url = url, Lang = lang });
}

