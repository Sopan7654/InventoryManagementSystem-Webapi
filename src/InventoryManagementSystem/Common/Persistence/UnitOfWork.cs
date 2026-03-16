// Common/Persistence/UnitOfWork.cs  — Unit of Work Pattern
using System.Data;
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;

namespace InventoryManagementSystem.Common.Persistence
{
    /// <summary>
    /// Unit of Work: wraps a single MySqlConnection + MySqlTransaction.
    /// Inventory command handlers (StockIn, StockOut, Transfer …) inject IUnitOfWork
    /// to ensure all stock mutations and transaction-log inserts commit or rollback atomically.
    /// Registered as <b>Scoped</b> so one instance lives per HTTP request.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnectionFactory _factory;
        private MySqlConnection? _connection;
        private MySqlTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(IDbConnectionFactory factory) => _factory = factory;

        public MySqlConnection Connection
        {
            get
            {
                if (_connection is null)
                    _connection = _factory.CreateConnection();
                return _connection;
            }
        }

        public MySqlTransaction? Transaction => _transaction;

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (Connection.State != ConnectionState.Open)
                await Connection.OpenAsync(ct);

            _transaction = await Connection.BeginTransactionAsync(ct) as MySqlTransaction;
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction to commit.");
            await _transaction.CommitAsync(ct);
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_transaction is not null)
                await _transaction.RollbackAsync(ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            if (_transaction is not null)
                await _transaction.DisposeAsync();

            if (_connection is not null)
                await _connection.DisposeAsync();
        }
    }
}
