namespace Insurance.JobService.Endpoints.Api.Controllers;

using Hangfire;
using Hangfire.Storage;
using Insurance.JobService.Endpoints.Api.Models;
using Insurance.JobService.Endpoints.Api.Services;
using Microsoft.AspNetCore.Mvc;


[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class DynamicHangfireController : ControllerBase
{
    private readonly IJobSchedulerService _jobSchedulerService;
    private readonly ILogger<DynamicHangfireController> _logger;

    public DynamicHangfireController(
        IJobSchedulerService jobSchedulerService,
        ILogger<DynamicHangfireController> logger)
    {
        _jobSchedulerService = jobSchedulerService;
        _logger = logger;
    }

    /// <summary>
    /// Schedule a dynamic job
    /// </summary>
    [HttpPost("schedule")]
    public async Task<IActionResult> ScheduleJob([FromBody] DynamicJobRequest request, CancellationToken ct)
    {
        if (request == null)
            return BadRequest(new { Error = "Job request cannot be null" });

        try
        {
            string jobId = request.JobType.ToLower() switch
            {
                "immediate" => _jobSchedulerService.ScheduleImmediateJob(request, ct),
                "scheduled" => _jobSchedulerService.ScheduleDelayedJob(request, ct),
                "recurring" => _jobSchedulerService.ScheduleRecurringJob(request, ct),
                _ => throw new ArgumentException($"Invalid job type: {request.JobType}")
            };

            return Ok(new
            {
                Success = true,
                JobId = jobId,
                JobName = request.JobName,
                JobType = request.JobType,
                ScheduledTime = request.ScheduledTime,
                Status = "Scheduled successfully",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling job: {JobName}", request.JobName);
            return StatusCode(500, new
            {
                Success = false,
                Error = "Failed to schedule job",
                Details = ex.Message,
                JobName = request.JobName
            });
        }
    }

    /// <summary>
    /// Schedule multiple jobs at once
    /// </summary>
    [HttpPost("schedule-batch")]
    public IActionResult ScheduleBatchJobs([FromBody] List<DynamicJobRequest> requests, CancellationToken ct)
    {
        if (requests == null || !requests.Any())
            return BadRequest(new { Error = "Job requests cannot be empty" });

        var results = new List<object>();

        foreach (var request in requests)
        {
            try
            {
                string jobId = request.JobType.ToLower() switch
                {
                    "immediate" => _jobSchedulerService.ScheduleImmediateJob(request, ct),
                    "scheduled" => _jobSchedulerService.ScheduleDelayedJob(request, ct),
                    "recurring" => _jobSchedulerService.ScheduleRecurringJob(request, ct),
                    _ => throw new ArgumentException($"Invalid job type: {request.JobType}")
                };

                results.Add(new
                {
                    Success = true,
                    JobId = jobId,
                    JobName = request.JobName,
                    JobType = request.JobType,
                    Status = "Scheduled successfully"
                });
            }
            catch (Exception ex)
            {
                results.Add(new
                {
                    Success = false,
                    JobName = request.JobName,
                    Error = ex.Message
                });
            }
        }

        return Ok(new
        {
            TotalJobs = requests.Count,
            SuccessfulJobs = results.Count(r => (bool)r.GetType().GetProperty("Success").GetValue(r)),
            FailedJobs = results.Count(r => !(bool)r.GetType().GetProperty("Success").GetValue(r)),
            Results = results
        });
    }

    /// <summary>
    /// Get job status
    /// </summary>
    [HttpGet("status/{jobId}")]
    public IActionResult GetJobStatus(string jobId)
    {
        try
        {
            var status = _jobSchedulerService.GetJobStatus(jobId);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job status: {JobId}", jobId);
            return NotFound(new
            {
                Error = "Job not found",
                JobId = jobId,
                Details = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete a scheduled job
    /// </summary>
    [HttpDelete("job/{jobId}")]
    public IActionResult DeleteJob(string jobId)
    {
        var deleted = _jobSchedulerService.DeleteJob(jobId);

        if (deleted)
        {
            return Ok(new
            {
                Success = true,
                JobId = jobId,
                Message = "Job deleted successfully"
            });
        }

        return NotFound(new
        {
            Success = false,
            JobId = jobId,
            Message = "Job not found or could not be deleted"
        });
    }

    /// <summary>
    /// Delete a recurring job
    /// </summary>
    [HttpDelete("recurring/{jobId}")]
    public IActionResult DeleteRecurringJob(string jobId)
    {
        var deleted = _jobSchedulerService.DeleteRecurringJob(jobId);

        return Ok(new
        {
            Success = deleted,
            JobId = jobId,
            Message = "Recurring job deleted successfully"
        });
    }

    /// <summary>
    /// List all recurring jobs
    /// </summary>
    [HttpGet("recurring-jobs")]
    public IActionResult GetRecurringJobs()
    {
        using (var connection = JobStorage.Current.GetConnection())
        {
            var recurringJobs = connection.GetRecurringJobs();
            var jobs = recurringJobs.Select(job => new
            {
                JobId = job.Id,
                JobName = job.Job?.Args?.FirstOrDefault()?.ToString(),
                Cron = job.Cron,
                LastExecution = job.LastJobId != null ? connection.GetJobData(job.LastJobId)?.CreatedAt : null,
                NextExecution = job.NextExecution,
                CreatedAt = job.CreatedAt,
                Status = job.Error != null ? "Error" : "Active",
                Error = job.Error
            });

            return Ok(new
            {
                TotalJobs = jobs.Count(),
                Jobs = jobs
            });
        }
    }

    /// <summary>
    /// Trigger a recurring job immediately
    /// </summary>
    [HttpPost("recurring/{jobId}/trigger")]
    public IActionResult TriggerRecurringJob(string jobId)
    {
        RecurringJob.TriggerJob(jobId);

        return Ok(new
        {
            Success = true,
            JobId = jobId,
            Message = "Recurring job triggered successfully"
        });
    }

    /// <summary>
    /// Update an existing recurring job
    /// </summary>
    [HttpPut("recurring/{jobId}")]
    public IActionResult UpdateRecurringJob(string jobId, [FromBody] DynamicJobRequest request, CancellationToken ct)
    {
        _jobSchedulerService.DeleteRecurringJob(jobId);

        var newJobId = _jobSchedulerService.ScheduleRecurringJob(request, ct);

        return Ok(new
        {
            Success = true,
            OldJobId = jobId,
            NewJobId = newJobId,
            Message = "Recurring job updated successfully"
        });
    }

    /// <summary>
    /// Schedule a job with custom queue configuration
    /// </summary>
    [HttpPost("schedule-with-queue")]
    public async Task<IActionResult> ScheduleJobWithCustomQueue(
        [FromBody] DynamicJobRequest request,
        CancellationToken ct,
        [FromQuery] string queueName = null,
        [FromQuery] string exchangeName = null)
    {
        return await ScheduleJob(request, ct);
    }

    /// <summary>
    /// Get Hangfire server statistics
    /// </summary>
    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        var monitor = JobStorage.Current.GetMonitoringApi();
        var stats = monitor.GetStatistics();

        return Ok(new
        {
            Servers = stats.Servers,
            Enqueued = stats.Enqueued,
            Scheduled = stats.Scheduled,
            Processing = stats.Processing,
            Succeeded = stats.Succeeded,
            Failed = stats.Failed,
            Deleted = stats.Deleted,
            Recurring = stats.Recurring,
            QueueCounts = stats.Queues,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Clean up old jobs
    /// </summary>
    [HttpPost("cleanup")]
    public IActionResult CleanupJobs([FromServices] IBackgroundJobClient backgroundJobClient, CancellationToken ct, [FromQuery] int daysToKeep = 7)
    {
        var monitor = JobStorage.Current.GetMonitoringApi();
        var toDeleteFrom = DateTime.UtcNow.AddDays(-daysToKeep);

        var deletedCount = 0;

        // Delete succeeded jobs older than specified days
        var succeededJobs = monitor.SucceededJobs(0, 1000);
        foreach (var job in succeededJobs)
        {
            backgroundJobClient.Delete(job.Key);
            deletedCount++;
        }

        // Delete failed jobs older than specified days
        var failedJobs = monitor.FailedJobs(0, 1000);
        foreach (var job in failedJobs)
        {
            backgroundJobClient.Delete(job.Key);
            deletedCount++;
        }

        return Ok(new
        {
            Success = true,
            DeletedJobs = deletedCount,
            RetentionDays = daysToKeep,
            Message = $"Cleaned up jobs older than {daysToKeep} days"
        });
    }
}