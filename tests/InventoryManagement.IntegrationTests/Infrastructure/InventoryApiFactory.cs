// ============================================================
// FILE: tests/InventoryManagement.IntegrationTests/Infrastructure/InventoryApiFactory.cs
// ============================================================
using InventoryManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace InventoryManagement.IntegrationTests.Infrastructure
{
    public class InventoryApiFactory : WebApplicationFactory<Program>
    {
        private DbConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<InventoryDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);

                var cacheDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(InventoryManagement.Application.Interfaces.ICacheService));
                if (cacheDescriptor != null) services.Remove(cacheDescriptor);
                
                var blacklistDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(InventoryManagement.Application.Interfaces.ITokenBlacklistService));
                if (blacklistDescriptor != null) services.Remove(blacklistDescriptor);

                services.AddSingleton(Moq.Mock.Of<InventoryManagement.Application.Interfaces.ICacheService>());
                services.AddSingleton(Moq.Mock.Of<InventoryManagement.Application.Interfaces.ITokenBlacklistService>());

                if (_connection == null)
                {
                    _connection = new SqliteConnection("DataSource=:memory:");
                    _connection.Open();
                }

                services.AddDbContext<InventoryDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
            });

            builder.UseEnvironment("Development");
        }

        protected override Microsoft.Extensions.Hosting.IHost CreateHost(Microsoft.Extensions.Hosting.IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                db.Database.EnsureCreated();
                DatabaseSeeder.SeedData(db);
            }

            return host;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection?.Dispose();
        }
    }
}
