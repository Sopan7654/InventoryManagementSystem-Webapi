// Common/Persistence/DbConnectionFactory.cs  — Singleton + Factory Pattern
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;

namespace InventoryManagementSystem.Common.Persistence
{
    /// <summary>
    /// Singleton Pattern: registered as Singleton in DI so the connection string is
    /// resolved from IConfiguration exactly once at application startup.
    ///
    /// Factory Pattern: <see cref="CreateConnection"/> produces a new
    /// <see cref="MySqlConnection"/> on every call — cheap to create, caller owns lifetime.
    /// </summary>
    public sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventoryDB")
                ?? throw new InvalidOperationException(
                    "Required connection string 'InventoryDB' is missing from appsettings.json.");
        }

        /// <summary>Creates a new (not yet opened) MySqlConnection.</summary>
        public MySqlConnection CreateConnection() => new(_connectionString);
    }
}
