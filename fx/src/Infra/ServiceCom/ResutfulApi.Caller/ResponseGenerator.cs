namespace OysterFx.Infra.ServiceCom.ResutfulApi.Caller;

public static class ResponseGenerator<TData>
{
    public static ApplicationServiceCommandResponse<TData> SuccessResponse(TData data)
    {
        return new ApplicationServiceCommandResponse<TData>()
        {
            Data = data,
            StatusCode = 200
        };
    }

    public static ApplicationServiceCommandResponse<TData> BadRequestResponse(TData data, List<string> errors)
    {
        var response = new ApplicationServiceCommandResponse<TData>()
        {
            Data = default,
            StatusCode = 400
        };

        if (errors != null && errors.Any())
        {
            foreach (var item in errors)
            {
                response.AddMessage(item);
            }
        }
        return response;
    }
}