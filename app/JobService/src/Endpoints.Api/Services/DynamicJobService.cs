namespace Insurance.JobService.Endpoints.Api.Services;

using Insurance.JobService.Endpoints.Api.Exceptions;
using Insurance.JobService.Endpoints.Api.Models;
using Newtonsoft.Json;
using OysterFx.Infra.EventBus.Outbox;
using System.Diagnostics;

public class DynamicJobService : IDynamicJobService
{
    private readonly ILogger<DynamicJobService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOutboxStore _outboxStore;

    public DynamicJobService(
        ILogger<DynamicJobService> logger,
        IServiceProvider serviceProvider,
        IOutboxStore outboxStore)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _outboxStore = outboxStore;
    }

    public async Task<JobExecutionResult> ExecuteJobAsync(DynamicJobRequest jobRequest, CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var jobId = Guid.NewGuid().ToString();

        _logger.LogInformation("Starting dynamic job execution. JobId: {JobId}, JobName: {JobName}",
            jobId, jobRequest.JobName);

        var result = new JobExecutionResult
        {
            JobId = jobId,
            JobName = jobRequest.JobName,
            ExecutionTime = DateTime.UtcNow,
            Status = JobStatus.Running,
            Metadata = new Dictionary<string, object>
                {
                    { "JobType", jobRequest.JobType },
                    { "ScheduledTime", jobRequest.ScheduledTime },
                    { "CronExpression", jobRequest.CronExpression }
                }
        };

        try
        {
            result.Status = JobStatus.Completed;
            result.Duration = stopwatch.Elapsed;

            var payload = new ScheduledMessageDto()
            {
                JobId = jobId,
                ScheduledTime = jobRequest.ScheduledTime,
                ExecutionTime = DateTime.UtcNow,
                Status = "Scheduled successfully",
                Metadata = new()
            };
            var metadata = new Dictionary<string, object>
                {
                    { "JobType", jobRequest.JobType },
                    { "ScheduledTime", jobRequest.ScheduledTime },
                    { "CronExpression", jobRequest.CronExpression }
                };

            await _outboxStore.AddAsync(new()
            {
                EventType = "",
                Payload = JsonConvert.SerializeObject(payload),
                OccurredAtUtc = DateTime.UtcNow,
                Metadata = JsonConvert.SerializeObject(metadata)
            }, ct);

            _logger.LogInformation("Job completed successfully. JobId: {JobId}, Duration: {Duration}ms",
                jobId, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            result.Status = JobStatus.Failed;
            result.ErrorMessage = ex.Message;
            result.Duration = stopwatch.Elapsed;

            _logger.LogError(ex, "Job execution failed. JobId: {JobId}, JobName: {JobName}",
                jobId, jobRequest.JobName);

            // TODO: Send to DLQ

            throw new JobExecutionException($"Job execution failed: {ex.Message}", ex);
        }

        return result;
    }
}