namespace OysterFx.Infra.ServiceCom.ResutfulApi.Caller;

public class ApiResultModel<TData>
{
    private ApiResultModel(TData data, List<string> errors)
    {
        Data = data;
        Errors = errors;
    }
    public TData Data { get; set; }
    public List<string> Errors { get; set; }

    public static ApiResultModel<TData> GenerateResult(TData data, List<string> errors)
    {
        return new ApiResultModel<TData>(data, errors);
    }

    public static ApiResultModel<TData> GenerateInputInvalidResult(TData data)
    {
        var errors = new List<string>()
            {
                "Invalid inputs"
            };

        var result = GenerateResult(data, errors);

        return result;
    }
    public bool HasError()
    {
        return Errors != null && Errors.Any();
    }
}