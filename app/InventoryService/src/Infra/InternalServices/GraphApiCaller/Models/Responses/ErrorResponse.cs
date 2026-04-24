namespace Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models.Responses;

public sealed class ErrorResponse
{
    public List<string> Errors { get; set; } = new();
}
