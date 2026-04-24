namespace OysterFx.AppCore.Shared.Commands.Common;

public class CommandResult<T>
{
    public bool IsSuccess { get; }
    public T Data { get; }
    public List<string> ErrorMessages { get; }
    public Dictionary<string, string[]> ValidationErrors { get; }

    protected CommandResult(
        bool isSuccess,
        T data = default!,
        List<string> errorMessages = null,
        Dictionary<string, string[]> validationErrors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessages = errorMessages ?? new List<string>();
        ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();
    }

    // Success cases
    public static CommandResult<T> Success(T data) => new CommandResult<T>(true, data);

    // Failure cases with single error message (maintains backward compatibility)
    public static CommandResult<T> Failure(string errorMessage) =>
        new CommandResult<T>(false, default!, new List<string> { errorMessage });

    public static CommandResult<T> Failure(string errorMessage, T data) =>
        new CommandResult<T>(false, data, new List<string> { errorMessage });

    // New failure cases with multiple error messages
    public static CommandResult<T> Failure(List<string> errorMessages) =>
        new CommandResult<T>(false, default!, errorMessages);

    public static CommandResult<T> Failure(List<string> errorMessages, T data) =>
        new CommandResult<T>(false, data, errorMessages);

    // Validation failure
    public static CommandResult<T> ValidationFailure(Dictionary<string, string[]> validationErrors) =>
        new CommandResult<T>(false, default!, new List<string> { "Validation failed" }, validationErrors);

    // Default failure
    public static CommandResult<T> Default() => new CommandResult<T>(false);

    // Conversion from non-generic CommandResult
    public static implicit operator CommandResult<T>(CommandResult result)
    {
        if (result.IsSuccess)
            return Success(default!);

        return result.ValidationErrors?.Count > 0
            ? ValidationFailure(result.ValidationErrors)
            : Failure(result.ErrorMessages ?? new List<string>());
    }

    // Helper property for backward compatibility (single error message)
    [Obsolete("Use ErrorMessages instead for multiple error support")]
    public string ErrorMessage => ErrorMessages.FirstOrDefault();

    // Helper method to add errors
    public void AddError(string error)
    {
        ErrorMessages.Add(error);
    }

    // Helper method to add multiple errors
    public void AddErrors(IEnumerable<string> errors)
    {
        ErrorMessages.AddRange(errors);
    }
}

// Non-generic version for completeness
public class CommandResult
{
    public bool IsSuccess { get; }
    public List<string> ErrorMessages { get; }
    public Dictionary<string, string[]> ValidationErrors { get; }

    protected CommandResult(
        bool isSuccess,
        List<string> errorMessages = null,
        Dictionary<string, string[]> validationErrors = null)
    {
        IsSuccess = isSuccess;
        ErrorMessages = errorMessages ?? new List<string>();
        ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();
    }

    // Default failure
    public static CommandResult Default() => new CommandResult(false);

    public static CommandResult Success() => new CommandResult(true);
    public static CommandResult Failure(string errorMessage) => new CommandResult(false, new List<string> { errorMessage });
    public static CommandResult Failure(List<string> errorMessages) => new CommandResult(false, errorMessages);
    public static CommandResult ValidationFailure(Dictionary<string, string[]> validationErrors) =>
        new CommandResult(false, new List<string> { "Validation failed" }, validationErrors);

    [Obsolete("Use ErrorMessages instead for multiple error support")]
    public string ErrorMessage => ErrorMessages.FirstOrDefault();
}