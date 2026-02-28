// Data/InventoryDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace InventoryManagementSystem.Data
{
    /// <summary>
    /// Design-time factory used by EF Core tooling (dotnet ef migrations add / update).
    /// Reads the connection string from appsettings.json so no app startup is needed.
    /// </summary>
    public class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
    {
        public InventoryDbContext CreateDbContext(string[] args)
        {
            // Resolve appsettings.json relative to the project root
            string projectDir = Directory.GetCurrentDirectory();
            string configPath = Path.Combine(projectDir, "appsettings.json");

            if (!File.Exists(configPath))
                throw new FileNotFoundException($"appsettings.json not found at: {configPath}");

            string json = File.ReadAllText(configPath);
            using var doc = JsonDocument.Parse(json);
            string connectionString = doc.RootElement
                .GetProperty("ConnectionStrings")
                .GetProperty("InventoryDB")
                .GetString()
                ?? throw new InvalidOperationException("InventoryDB connection string is missing.");

            var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();

            // Pomelo MySQL provider — auto-detect server version from connection string
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions
                    .MigrationsHistoryTable("__EFMigrationsHistory")
                    .EnableRetryOnFailure(maxRetryCount: 3)
            );

            return new InventoryDbContext(optionsBuilder.Options);
        }
    }
}
