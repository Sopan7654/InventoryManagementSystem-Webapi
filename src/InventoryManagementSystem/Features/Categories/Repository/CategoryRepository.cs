// Features/Categories/Repository/CategoryRepository.cs
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Categories.Repository
{
    public sealed class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnectionFactory _factory;
        public CategoryRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<List<ProductCategory>> GetAllAsync(CancellationToken ct = default)
        {
            var list = new List<ProductCategory>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT c.CategoryId, c.CategoryName, c.Description,
                                        c.ParentCategoryId, p.CategoryName AS ParentCategoryName
                                 FROM ProductCategory c
                                 LEFT JOIN ProductCategory p ON p.CategoryId = c.ParentCategoryId
                                 ORDER BY c.ParentCategoryId, c.CategoryName";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        public async Task<ProductCategory?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT c.CategoryId, c.CategoryName, c.Description,
                                        c.ParentCategoryId, p.CategoryName AS ParentCategoryName
                                 FROM ProductCategory c
                                 LEFT JOIN ProductCategory p ON p.CategoryId = c.ParentCategoryId
                                 WHERE c.CategoryId = @id";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            return await rdr.ReadAsync(ct) ? Map(rdr) : null;
        }

        public async Task InsertAsync(ProductCategory c, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = "INSERT INTO ProductCategory (CategoryId,CategoryName,Description,ParentCategoryId) VALUES (@id,@name,@desc,@parent)";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id",     c.CategoryId);
            cmd.Parameters.AddWithValue("@name",   c.CategoryName);
            cmd.Parameters.AddWithValue("@desc",   c.Description        ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@parent", c.ParentCategoryId   ?? (object)DBNull.Value);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static ProductCategory Map(MySqlDataReader r) => new()
        {
            CategoryId         = r["CategoryId"].ToString()!,
            CategoryName       = r["CategoryName"].ToString()!,
            Description        = r["Description"]        == DBNull.Value ? null : r["Description"].ToString(),
            ParentCategoryId   = r["ParentCategoryId"]   == DBNull.Value ? null : r["ParentCategoryId"].ToString(),
            ParentCategoryName = r["ParentCategoryName"] == DBNull.Value ? null : r["ParentCategoryName"].ToString()
        };
    }
}
