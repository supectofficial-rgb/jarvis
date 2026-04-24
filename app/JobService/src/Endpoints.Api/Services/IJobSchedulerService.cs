namespace Insurance.JobService.Endpoints.Api.Services;

using Insurance.JobService.Endpoints.Api.Models;

public interface IJobSchedulerService
{
    string ScheduleImmediateJob(DynamicJobRequest request, CancellationToken ct);
    string ScheduleDelayedJob(DynamicJobRequest request, CancellationToken ct);
    string ScheduleRecurringJob(DynamicJobRequest request, CancellationToken ct);
    bool DeleteJob(string jobId);
    bool DeleteRecurringJob(string jobId);
    JobExecutionResult GetJobStatus(string jobId);
}