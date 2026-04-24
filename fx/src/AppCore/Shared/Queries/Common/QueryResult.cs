namespace OysterFx.AppCore.Shared.Queries.Common
{
    public class QueryResult<TData>
    {
        public bool IsSuccess { get; }
        public TData Data { get; }
        public string ErrorMessage { get; }
        public string ErrorCode { get; }  // Machine-readable error code (e.g., "NOT_FOUND")
        public IEnumerable<string> ValidationErrors { get; }  // Detailed validation issues

        // Private constructor (immutable)
        protected QueryResult(
            bool isSuccess,
            TData data = default,
            string errorMessage = null,
            string errorCode = null,
            IEnumerable<string> validationErrors = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            ValidationErrors = validationErrors ?? Enumerable.Empty<string>();
        }

        // Success with data
        public static QueryResult<TData> Success(TData data)
            => new QueryResult<TData>(true, data);

        // Failure with error message/code
        public static QueryResult<TData> Fail(
            string errorMessage,
            string errorCode = null,
            IEnumerable<string> validationErrors = null)
            => new QueryResult<TData>(false, default, errorMessage, errorCode, validationErrors);

        public static QueryResult<TData> Default() => new QueryResult<TData>(false);

        // Implicit bool conversion (for quick checks)
        public static implicit operator bool(QueryResult<TData> result) => result.IsSuccess;
    }
}
