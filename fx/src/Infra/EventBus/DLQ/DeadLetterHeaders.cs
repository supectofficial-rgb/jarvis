namespace OysterFx.Infra.EventBus.DLQ;

public static class DeadLetterHeaders
{
    public const string Error = "x-error";
    public const string FailedService = "x-failed-service";
    public const string OriginalEvent = "x-original-event";
}