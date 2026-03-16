// Common/Models/Result.cs
namespace InventoryManagementSystem.Common.Models
{
    /// <summary>
    /// Result monad — replaces (bool Success, string Message) tuples.
    /// All CQRS handlers return Result&lt;T&gt; so controllers never deal with raw exceptions.
    /// </summary>
    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string Error { get; }

        private Result(bool isSuccess, T? value, string error)
        {
            IsSuccess = isSuccess;
            Value     = value;
            Error     = error;
        }

        public static Result<T> Success(T value)        => new(true,  value,   string.Empty);
        public static Result<T> Failure(string error)   => new(false, default, error);

        public static implicit operator Result<T>(T value) => Success(value);
    }
}
