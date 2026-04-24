namespace Insurance.JobService.Endpoints.Api.Services;

using Hangfire;
using Insurance.JobService.Endpoints.Api.Models;

public class JobSchedulerService : IJobSchedulerService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IDynamicJobService _dynamicJobService;
    private readonly ILogger<JobSchedulerService> _logger;

    public JobSchedulerService(
        IBackgroundJobClient backgroundJobClient,
        IRecurringJobManager recurringJobManager,
        IDynamicJobService dynamicJobService,
        ILogger<JobSchedulerService> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
        _dynamicJobService = dynamicJobService;
        _logger = logger;
    }

    public string ScheduleImmediateJob(DynamicJobRequest request, CancellationToken ct)
    {
        var jobId = _backgroundJobClient.Enqueue(() =>
            _dynamicJobService.ExecuteJobAsync(request, ct));

        _logger.LogInformation("Immediate job scheduled. JobId: {JobId}, JobName: {JobName}",
            jobId, request.JobName);

        return jobId;
    }

    public string ScheduleDelayedJob(DynamicJobRequest request, CancellationToken ct)
    {
        if (!request.ScheduledTime.HasValue)
            throw new ArgumentException("Scheduled time is required for delayed jobs");

        var delay = request.ScheduledTime.Value - DateTime.UtcNow;

        if (delay <= TimeSpan.Zero)
            return ScheduleImmediateJob(request, ct);

        var jobId = _backgroundJobClient.Schedule(() =>
            _dynamicJobService.ExecuteJobAsync(request, ct), delay);

        _logger.LogInformation("Delayed job scheduled. JobId: {JobId}, JobName: {JobName}, ScheduledTime: {ScheduledTime}",
            jobId, request.JobName, request.ScheduledTime);

        return jobId;
    }

    public string ScheduleRecurringJob(DynamicJobRequest request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.CronExpression))
            throw new ArgumentException("Cron expression is required for recurring jobs");

        var jobId = $"recurring-job-{request.JobName}-{Guid.NewGuid():N}";

        _recurringJobManager.AddOrUpdate(
            jobId,
            () => _dynamicJobService.ExecuteJobAsync(request, ct),
            request.CronExpression);

        _logger.LogInformation("Recurring job scheduled. JobId: {JobId}, JobName: {JobName}, Cron: {CronExpression}",
            jobId, request.JobName, request.CronExpression);

        return jobId;
    }

    public bool DeleteJob(string jobId)
    {
        var deleted = _backgroundJobClient.Delete(jobId);
        _logger.LogInformation("Job deletion attempt. JobId: {JobId}, Success: {Success}", jobId, deleted);
        return deleted;
    }

    public bool DeleteRecurringJob(string jobId)
    {
        _recurringJobManager.RemoveIfExists(jobId);
        _logger.LogInformation("Recurring job removed. JobId: {JobId}", jobId);
        return true;
    }

    public JobExecutionResult GetJobStatus(string jobId)
    {
        using (var connection = JobStorage.Current.GetConnection())
        {
            var jobData = connection.GetJobData(jobId);
            var state = connection.GetStateData(jobId);

            return new JobExecutionResult
            {
                JobId = jobId,
                Status = MapHangfireState(state?.Name),
                ExecutionTime = jobData?.CreatedAt ?? DateTime.UtcNow,
                Result = jobData?.Job?.Args,
                ErrorMessage = state?.Reason
            };
        }
    }

    private JobStatus MapHangfireState(string state)
    {
        return state?.ToLower() switch
        {
            "enqueued" => JobStatus.Pending,
            "processing" => JobStatus.Running,
            "succeeded" => JobStatus.Completed,
            "failed" => JobStatus.Failed,
            "deleted" => JobStatus.Completed,
            "scheduled" => JobStatus.Pending,
            _ => JobStatus.Pending
        };
    }
}