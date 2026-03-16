// Features/Products/Repository/ProductRepository.cs
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Products.Repository
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _factory;

        public ProductRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<List<Product>> GetAllAsync(CancellationToken ct = default)
        {
            var list = new List<Product>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT p.ProductId, p.SKU, p.ProductName, p.Description,
                                        p.CategoryId, c.CategoryName, p.UnitOfMeasure,
                                        p.Cost, p.ListPrice, p.IsActive
                                 FROM Product p
                                 LEFT JOIN ProductCategory c ON c.CategoryId = p.CategoryId
                                 ORDER BY p.ProductName";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        public async Task<Product?> GetByIdAsync(string productId, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT p.ProductId, p.SKU, p.ProductName, p.Description,
                                        p.CategoryId, c.CategoryName, p.UnitOfMeasure,
                                        p.Cost, p.ListPrice, p.IsActive
                                 FROM Product p
                                 LEFT JOIN ProductCategory c ON c.CategoryId = p.CategoryId
                                 WHERE p.ProductId = @id";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", productId);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            return await rdr.ReadAsync(ct) ? Map(rdr) : null;
        }

        public async Task<Product?> GetBySKUAsync(string sku, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT p.ProductId, p.SKU, p.ProductName, p.Description,
                                        p.CategoryId, c.CategoryName, p.UnitOfMeasure,
                                        p.Cost, p.ListPrice, p.IsActive
                                 FROM Product p
                                 LEFT JOIN ProductCategory c ON c.CategoryId = p.CategoryId
                                 WHERE p.SKU = @sku";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sku", sku);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            return await rdr.ReadAsync(ct) ? Map(rdr) : null;
        }

        public async Task InsertAsync(Product p, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"INSERT INTO Product
                                    (ProductId,SKU,ProductName,Description,
                                     CategoryId,UnitOfMeasure,Cost,ListPrice,IsActive)
                                 VALUES (@id,@sku,@name,@desc,@cat,@uom,@cost,@price,@active)";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id",     p.ProductId);
            cmd.Parameters.AddWithValue("@sku",    p.SKU);
            cmd.Parameters.AddWithValue("@name",   p.ProductName);
            cmd.Parameters.AddWithValue("@desc",   p.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@cat",    p.CategoryId  ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@uom",    p.UnitOfMeasure);
            cmd.Parameters.AddWithValue("@cost",   p.Cost);
            cmd.Parameters.AddWithValue("@price",  p.ListPrice);
            cmd.Parameters.AddWithValue("@active", p.IsActive);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task<bool> UpdateAsync(Product p, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"UPDATE Product
                                 SET ProductName=@name, Description=@desc,
                                     CategoryId=@cat, UnitOfMeasure=@uom,
                                     Cost=@cost, ListPrice=@price, IsActive=@active
                                 WHERE ProductId=@id";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name",   p.ProductName);
            cmd.Parameters.AddWithValue("@desc",   p.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@cat",    p.CategoryId  ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@uom",    p.UnitOfMeasure);
            cmd.Parameters.AddWithValue("@cost",   p.Cost);
            cmd.Parameters.AddWithValue("@price",  p.ListPrice);
            cmd.Parameters.AddWithValue("@active", p.IsActive);
            cmd.Parameters.AddWithValue("@id",     p.ProductId);
            return await cmd.ExecuteNonQueryAsync(ct) > 0;
        }

        public async Task<bool> SKUExistsAsync(string sku, string? excludeId = null,
            CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = "SELECT COUNT(*) FROM Product WHERE SKU=@sku AND ProductId != @excl";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sku",  sku);
            cmd.Parameters.AddWithValue("@excl", excludeId ?? string.Empty);
            var count = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt32(count) > 0;
        }

        private static Product Map(MySqlDataReader r) => new()
        {
            ProductId     = r["ProductId"].ToString()!,
            SKU           = r["SKU"].ToString()!,
            ProductName   = r["ProductName"].ToString()!,
            Description   = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
            CategoryId    = r["CategoryId"]  == DBNull.Value ? null : r["CategoryId"].ToString(),
            CategoryName  = r["CategoryName"]== DBNull.Value ? null : r["CategoryName"].ToString(),
            UnitOfMeasure = r["UnitOfMeasure"].ToString()!,
            Cost          = Convert.ToDecimal(r["Cost"]),
            ListPrice     = Convert.ToDecimal(r["ListPrice"]),
            IsActive      = Convert.ToBoolean(r["IsActive"])
        };
    }
}
