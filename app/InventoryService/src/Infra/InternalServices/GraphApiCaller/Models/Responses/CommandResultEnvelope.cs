namespace Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models.Responses;

public sealed class CommandResultEnvelope<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
}
