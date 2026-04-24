namespace Insurance.UserService.AppCore.Domain.OtpCodes.Entities;

using Insurance.UserService.AppCore.Domain.OtpCodes.Dtos;
using Insurance.UserService.AppCore.Domain.OtpCodes.Enums;
using OysterFx.AppCore.Domain.Aggregates;
using System;

public sealed class OtpCode : AggregateRoot
{
    public string? MobileNumber { get; private set; }
    public string Code { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsVerified { get; private set; }
    public int Attempts { get; private set; }
    public OtpStatus Status { get; private set; }

    private OtpCode() { } // For EF Core

    private OtpCode(
        string mobileNumber,
        string code,
        DateTime createdAt,
        DateTime expiresAt)
    {
        MobileNumber = mobileNumber;
        Code = code;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IsVerified = false;
        Attempts = 0;
        Status = OtpStatus.Active;
    }

    public static OtpCode Create(
         string mobileNumber,
        string code,
        TimeSpan expiration) => new OtpCode(
            mobileNumber,
            code,
            DateTime.UtcNow,
            DateTime.UtcNow.Add(expiration));

    public VerificationResult Verify(string code)
    {
        if (Status != OtpStatus.Active)
            return VerificationResult.Failed("OTP is not active");

        if (DateTime.UtcNow > ExpiresAt)
        {
            Status = OtpStatus.Expired;
            return VerificationResult.Failed("OTP has expired");
        }

        Attempts++;

        if (Code != code?.Trim())
        {
            if (Attempts >= 3)
            {
                Status = OtpStatus.Blocked;
                return VerificationResult.Failed("Maximum attempts exceeded");
            }

            return VerificationResult.Failed($"Invalid code. {3 - Attempts} attempts remaining");
        }

        IsVerified = true;
        Status = OtpStatus.Verified;

        return VerificationResult.Success();
    }

    public bool IsValid()
    {
        return Status == OtpStatus.Active &&
               DateTime.UtcNow <= ExpiresAt &&
               Attempts < 3;
    }

    public TimeSpan GetRemainingTime()
    {
        var remaining = ExpiresAt - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    public bool IsExpired()
    {
        return Status == OtpStatus.Expired;
    }
}