namespace Insurance.JobService.Endpoints.Api.Exceptions;

public class JobExecutionException : Exception
{
    public JobExecutionException(string message, Exception ex) : base(message, ex)
    {
    }
}