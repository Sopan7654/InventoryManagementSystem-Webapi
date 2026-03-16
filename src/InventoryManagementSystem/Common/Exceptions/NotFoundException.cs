// Common/Exceptions/NotFoundException.cs
namespace InventoryManagementSystem.Common.Exceptions
{
    /// <summary>Thrown when a requested resource does not exist — maps to HTTP 404.</summary>
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with key '{key}' was not found.") { }

        public NotFoundException(string message) : base(message) { }
    }
}
