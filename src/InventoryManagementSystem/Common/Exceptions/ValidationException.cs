// Common/Exceptions/ValidationException.cs
namespace InventoryManagementSystem.Common.Exceptions
{
    /// <summary>Thrown by ValidationBehavior when FluentValidation finds errors — maps to HTTP 400.</summary>
    public sealed class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ValidationException(IReadOnlyDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }
}
