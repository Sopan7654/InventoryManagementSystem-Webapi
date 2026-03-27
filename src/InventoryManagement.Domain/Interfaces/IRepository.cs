// ============================================================
// FILE: src/InventoryManagement.Domain/Interfaces/IRepository.cs
// ============================================================
using System.Linq.Expressions;

namespace InventoryManagement.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface providing standard CRUD operations.
    /// </summary>
    /// <typeparam name="T">The domain entity type.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>Gets all entities.</summary>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets an entity by its primary key.</summary>
        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>Finds entities matching a predicate.</summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>Adds a new entity.</summary>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>Updates an existing entity.</summary>
        void Update(T entity);

        /// <summary>Removes an entity.</summary>
        void Delete(T entity);

        /// <summary>Gets a paginated list of entities.</summary>
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            CancellationToken cancellationToken = default);
    }
}
