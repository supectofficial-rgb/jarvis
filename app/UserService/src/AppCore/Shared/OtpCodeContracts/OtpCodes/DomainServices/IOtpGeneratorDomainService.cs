namespace Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.DomainServices;

public interface IOtpGeneratorDomainService
{
    public Task<string> ExecuteAsync(string mobileNumber, int length = 5);
}