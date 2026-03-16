// Common/Interfaces/IRepository.cs  — Repository Pattern (generic base)
namespace InventoryManagementSystem.Common.Interfaces
{
    /// <summary>
    /// Generic marker interface for the Repository Pattern.
    /// Feature-specific repositories extend this with domain-specific operations.
    /// Follows Interface Segregation Principle — keeps the base minimal.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        // Intentionally minimal — each feature repo declares its own async contract
    }
}
