// ============================================================
// FILE: src/InventoryManagement.Shared/Result.cs
// ============================================================
namespace InventoryManagement.Shared
{
    /// <summary>
    /// Categorizes the type of error in a failed Result.
    /// </summary>
    public enum ResultError
    {
        /// <summary>No error.</summary>
        None,
        /// <summary>The requested resource was not found.</summary>
        NotFound,
        /// <summary>A conflict occurred (e.g., duplicate resource).</summary>
        Conflict,
        /// <summary>One or more validation errors occurred.</summary>
        Validation,
        /// <summary>The request is not authorized.</summary>
        Unauthorized,
        /// <summary>An internal server error occurred.</summary>
        ServerError
    }

    /// <summary>
    /// Represents the outcome of an operation, encapsulating success/failure,
    /// a value, an error message, and an error type.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    public class Result<T>
    {
        /// <summary>Whether the operation succeeded.</summary>
        public bool IsSuccess { get; }

        /// <summary>The result value (null on failure).</summary>
        public T? Value { get; }

        /// <summary>Error message (null on success).</summary>
        public string? Error { get; }

        /// <summary>Type of error for HTTP status code mapping.</summary>
        public ResultError ErrorType { get; }

        /// <summary>Validation errors (empty on success or non-validation errors).</summary>
        public IDictionary<string, string[]> ValidationErrors { get; }

        private Result(bool isSuccess, T? value, string? error, ResultError errorType,
            IDictionary<string, string[]>? validationErrors = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            ErrorType = errorType;
            ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();
        }

        /// <summary>Creates a successful result with a value.</summary>
        public static Result<T> Success(T value) =>
            new(true, value, null, ResultError.None);

        /// <summary>Creates a failure result with a not-found error.</summary>
        public static Result<T> NotFound(string error) =>
            new(false, default, error, ResultError.NotFound);

        /// <summary>Creates a failure result with a conflict error.</summary>
        public static Result<T> ConflictError(string error) =>
            new(false, default, error, ResultError.Conflict);

        /// <summary>Creates a failure result with a validation error.</summary>
        public static Result<T> ValidationError(string error,
            IDictionary<string, string[]>? validationErrors = null) =>
            new(false, default, error, ResultError.Validation, validationErrors);

        /// <summary>Creates a failure result with an unauthorized error.</summary>
        public static Result<T> Unauthorized(string error) =>
            new(false, default, error, ResultError.Unauthorized);

        /// <summary>Creates a failure result with a server error.</summary>
        public static Result<T> ServerError(string error) =>
            new(false, default, error, ResultError.ServerError);

        /// <summary>Creates a generic failure result.</summary>
        public static Result<T> Failure(string error, ResultError errorType = ResultError.ServerError) =>
            new(false, default, error, errorType);
    }

    /// <summary>
    /// Non-generic Result for operations with no return value.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public ResultError ErrorType { get; }

        private Result(bool isSuccess, string? error, ResultError errorType)
        {
            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }

        public static Result Success() => new(true, null, ResultError.None);
        public static Result NotFound(string error) => new(false, error, ResultError.NotFound);
        public static Result ConflictError(string error) => new(false, error, ResultError.Conflict);
        public static Result ValidationError(string error) => new(false, error, ResultError.Validation);
        public static Result Unauthorized(string error) => new(false, error, ResultError.Unauthorized);
        public static Result ServerError(string error) => new(false, error, ResultError.ServerError);
        public static Result Failure(string error, ResultError errorType = ResultError.ServerError) =>
            new(false, error, errorType);
    }
}
