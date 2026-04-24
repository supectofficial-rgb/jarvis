namespace Insurance.JobService.Endpoints.Api.Services;

using Insurance.JobService.Endpoints.Api.Models;

public interface IDynamicJobService
{
    Task<JobExecutionResult> ExecuteJobAsync(DynamicJobRequest jobRequest, CancellationToken ct);
}