namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

public interface IActionCatalogService
{
    Task<IReadOnlyList<ActionMetadata>> GetAllAsync(CancellationToken cancellationToken);
    Task<ActionMetadata?> FindByActionNameAsync(string actionName, CancellationToken cancellationToken);
}
