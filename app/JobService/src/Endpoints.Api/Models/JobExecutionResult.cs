namespace Insurance.JobService.Endpoints.Api.Models;

public class JobExecutionResult
{
    public string JobId { get; set; }
    public string JobName { get; set; }
    public DateTime ExecutionTime { get; set; }
    public JobStatus Status { get; set; }
    public object Result { get; set; }
    public string ErrorMessage { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
