namespace Insurance.JobService.Endpoints.Api;

public interface IJobScheduleExecuterService
{
    public Task<bool> ExecuteAsync();
}