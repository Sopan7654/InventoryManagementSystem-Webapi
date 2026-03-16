// Common/Exceptions/DomainException.cs
namespace InventoryManagementSystem.Common.Exceptions
{
    /// <summary>Thrown when a business/domain rule is violated (e.g. insufficient stock).</summary>
    public sealed class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception inner) : base(message, inner) { }
    }
}
