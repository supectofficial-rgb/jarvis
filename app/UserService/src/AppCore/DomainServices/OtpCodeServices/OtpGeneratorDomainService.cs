namespace Suspect.TaskPro.AppCore.DomainServices.OtpCodeServices;

using Insurance.UserService.AppCore.Domain.OtpCodes.Entities;
using Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.Commands;
using Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.DomainServices;
using System.Text;

public class OtpGeneratorDomainService : IOtpGeneratorDomainService
{
    private readonly Random _random = new();
    private readonly IOtpCodeCommandRepository _otpCodeCommandRepository;

    public OtpGeneratorDomainService(IOtpCodeCommandRepository otpCodeCommandRepository)
    {
        _otpCodeCommandRepository = otpCodeCommandRepository;
    }

    public async Task<string> ExecuteAsync(string mobileNumber, int length = 5)
    {
        var code = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            code.Append(_random.Next(0, 10));
        }

        var otpCode = OtpCode.Create(mobileNumber, code.ToString(), TimeSpan.FromMinutes(2));
        //await _otpCodeCommandRepository.InsertAsync(otpCode);
        //await _otpCodeCommandRepository.CommitAsync();

        return code.ToString();
    }
}