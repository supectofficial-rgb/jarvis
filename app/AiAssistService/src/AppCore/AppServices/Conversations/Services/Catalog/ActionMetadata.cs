namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

public sealed class ActionMetadata
{
    public string ActionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Aliases { get; set; } = new();
    public List<string> RequiredParams { get; set; } = new();
    public List<string> OptionalParams { get; set; } = new();
    public string PermissionKey { get; set; } = string.Empty;
    public bool IsAsync { get; set; }
    public bool ConfirmationRequired { get; set; }
    public bool Idempotent { get; set; } = true;
    public string DestinationService { get; set; } = string.Empty;
    public string EndpointMetadata { get; set; } = string.Empty;
}


