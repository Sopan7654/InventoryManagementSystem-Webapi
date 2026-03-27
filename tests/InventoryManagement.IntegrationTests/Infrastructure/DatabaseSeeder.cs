// ============================================================
// FILE: tests/InventoryManagement.IntegrationTests/Infrastructure/DatabaseSeeder.cs
// ============================================================
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Models;
using InventoryManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace InventoryManagement.IntegrationTests.Infrastructure
{
    public static class DatabaseSeeder
    {
        public static void SeedData(InventoryDbContext db)
        {
            if (db.Users.Any()) return;

            // Admin User
            var adminUser = new User
            {
                UserId = "U1",
                Username = "admin",
                Email = "admin@test.com",
                PasswordHash = HashPassword("Admin123!"),
                Role = UserRole.Admin
            };
            db.Users.Add(adminUser);

            // Seed Category
            var category = new ProductCategory { CategoryId = "C1", CategoryName = "Electronics" };
            db.Categories.Add(category);

            // Seed Products
            db.Products.Add(new Product { ProductId = "P1", SKU = "SKU-001", ProductName = "Laptop", CategoryId = "C1", ListPrice = 1200 });
            db.Products.Add(new Product { ProductId = "P2", SKU = "SKU-002", ProductName = "Mouse", CategoryId = "C1", ListPrice = 50 });

            // Seed Warehouse
            var warehouse = new Warehouse { WarehouseId = "W1", WarehouseName = "Main DC", Capacity = 10000 };
            db.Warehouses.Add(warehouse);

            db.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
