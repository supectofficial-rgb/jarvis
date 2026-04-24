namespace OysterFx.Infra.ServiceCom.ResutfulApi.Caller;

public class ApplicationServiceCommandResponse<TData>
{
    public TData Data { get; set; }
    public int StatusCode { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public void AddMessage(string message)
    {
        Errors.Add(message);
    }
}