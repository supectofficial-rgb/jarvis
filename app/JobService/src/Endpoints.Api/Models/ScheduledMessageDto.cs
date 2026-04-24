namespace Insurance.JobService.Endpoints.Api.Models;

public class ScheduledMessageDto
{
    public DateTime? ScheduledTime { get; set; }
    public DateTime ExecutionTime { get; set; }
    public string JobId { get; set; }
    public string Status { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}