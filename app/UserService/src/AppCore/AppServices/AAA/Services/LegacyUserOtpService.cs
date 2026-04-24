namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.UserService.AppCore.Domain.OtpCodes.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.Commands;
using System.Security.Cryptography;

public sealed class LegacyUserOtpService : ILegacyUserOtpService
{
    private const int OtpLength = 5;
    private const int OtpExpirationMinutes = 2;

    private readonly IOtpCodeCommandRepository _otpCodeCommandRepository;

    public LegacyUserOtpService(IOtpCodeCommandRepository otpCodeCommandRepository)
    {
        _otpCodeCommandRepository = otpCodeCommandRepository;
    }

    public async Task<(bool Success, string? Code, string? Error, TimeSpan? Expiration)> CreateAsync(string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            return (false, null, "INVALID_MOBILE", null);
        }

        var code = GenerateOtpCode(OtpLength);
        var otpCode = OtpCode.Create(mobileNumber.Trim(), code, TimeSpan.FromMinutes(OtpExpirationMinutes));

        await _otpCodeCommandRepository.InsertAsync(otpCode);
        await _otpCodeCommandRepository.CommitAsync();

        return (true, code, null, TimeSpan.FromMinutes(OtpExpirationMinutes));
    }

    public async Task<(bool Verified, string Message)> VerifyAsync(string mobileNumber, string code)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber) || string.IsNullOrWhiteSpace(code))
        {
            return (false, "INVALID_CODE");
        }

        var latest = await _otpCodeCommandRepository.GetLatestByMobileNumberAsync(mobileNumber.Trim());
        if (latest is null)
        {
            return (false, "INVALID_CODE");
        }

        var verification = latest.Verify(code.Trim());
        await _otpCodeCommandRepository.CommitAsync();

        return verification.IsSuccess
            ? (true, "VERIFIED")
            : (false, verification.Message);
    }

    private static string GenerateOtpCode(int length)
    {
        Span<byte> randomBytes = stackalloc byte[length];
        RandomNumberGenerator.Fill(randomBytes);

        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('0' + (randomBytes[i] % 10));
        }

        return new string(chars);
    }
}
