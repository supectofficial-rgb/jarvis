namespace Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.Commands;

using Insurance.UserService.AppCore.Domain.OtpCodes.Entities;
using System.Threading.Tasks;

public interface IOtpCodeCommandRepository
{
    Task<OtpCode?> GetByMobileNumberAndCode(string mobileNumber, string code);
    Task<OtpCode?> GetLatestByMobileNumberAsync(string mobileNumber);
    Task InsertAsync(OtpCode entity);
    Task<int> CommitAsync();
}
