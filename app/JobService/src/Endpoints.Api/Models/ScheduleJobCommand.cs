namespace Insurance.JobService.Endpoints.Api.Models;

public class ScheduleJobCommand
{
    public Guid ChannelBusinessId { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public bool SendMessageToQueue { get; set; } = true;
    public string QueueType { get; set; } = "RabbitMQ"; // RabbitMQ or ActiveMQ
    public Dictionary<string, object> AdditionalData { get; set; }
}