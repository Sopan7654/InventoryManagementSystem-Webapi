// Data/Seeds/AppUserTableMigration.cs
// This runs once at startup to ensure the AppUser table exists.
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;

namespace InventoryManagementSystem.Data.Seeds
{
    /// <summary>
    /// Ensures the AppUser table exists in the database.
    /// Runs automatically at startup via Program.cs.
    /// Safe to run multiple times (uses CREATE TABLE IF NOT EXISTS).
    /// </summary>
    public sealed class AppUserTableMigration
    {
        private readonly IDbConnectionFactory _factory;
        private readonly ILogger<AppUserTableMigration> _logger;

        public AppUserTableMigration(IDbConnectionFactory factory, ILogger<AppUserTableMigration> logger)
        {
            _factory = factory;
            _logger  = logger;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            try
            {
                await using var conn = _factory.CreateConnection();
                await conn.OpenAsync(ct);

                const string sql = @"
                    CREATE TABLE IF NOT EXISTS AppUser (
                        UserId       VARCHAR(36)  NOT NULL PRIMARY KEY,
                        Username     VARCHAR(100) NOT NULL UNIQUE,
                        Email        VARCHAR(255) NOT NULL UNIQUE,
                        PasswordHash VARCHAR(255) NOT NULL,
                        Role         VARCHAR(50)  NOT NULL DEFAULT 'User',
                        IsActive     TINYINT(1)   NOT NULL DEFAULT 1,
                        CreatedAt    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        INDEX idx_username (Username),
                        INDEX idx_email    (Email)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                await using var cmd = new MySqlCommand(sql, conn);
                await cmd.ExecuteNonQueryAsync(ct);
                _logger.LogInformation("AppUser table ensured.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create AppUser table.");
                throw;
            }
        }
    }
}
