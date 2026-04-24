namespace Insurance.UserService.AppCore.Domain.OtpCodes.Dtos;

public sealed record VerificationResult
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public int RemainingAttempts { get; }

    private VerificationResult(bool isSuccess, string message, int remainingAttempts)
    {
        IsSuccess = isSuccess;
        Message = message;
        RemainingAttempts = remainingAttempts;
    }

    public static VerificationResult Success()
        => new VerificationResult(true, "Verified successfully", 0);

    public static VerificationResult Failed(string message, int remainingAttempts = 0)
        => new VerificationResult(false, message, remainingAttempts);
}