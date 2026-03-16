// Common/Interfaces/IDbConnectionFactory.cs  — Factory Pattern
using MySql.Data.MySqlClient;

namespace InventoryManagementSystem.Common.Interfaces
{
    /// <summary>
    /// Factory Pattern: abstracts creation of database connections.
    /// Registered as Singleton so the connection string is resolved once at startup.
    /// </summary>
    public interface IDbConnectionFactory
    {
        MySqlConnection CreateConnection();
    }
}
