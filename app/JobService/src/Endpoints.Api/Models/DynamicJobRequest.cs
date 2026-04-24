namespace Insurance.JobService.Endpoints.Api.Models;

public class DynamicJobRequest
{
    public string JobName { get; set; }
    public string JobType { get; set; } // Immediate, Scheduled, Recurring
    public DateTime? ScheduledTime { get; set; }
    public string CronExpression { get; set; }
    public Dictionary<string, object> JobData { get; set; }
}
