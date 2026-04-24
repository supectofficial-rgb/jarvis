namespace Suspect.TaskPro.AppCore.Shared.AAA.Services;

public interface IJwtTokenGeneratorService
{
    Task<string?> ExecuteAsync(string? mobileNumber, string userId);
}