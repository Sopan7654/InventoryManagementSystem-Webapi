// Features/Warehouses/Repository/WarehouseRepository.cs
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Warehouses.Repository
{
    public sealed class WarehouseRepository : IWarehouseRepository
    {
        private readonly IDbConnectionFactory _factory;
        public WarehouseRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<List<Warehouse>> GetAllAsync(CancellationToken ct = default)
        {
            var list = new List<Warehouse>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            await using var cmd = new MySqlCommand("SELECT * FROM Warehouse ORDER BY WarehouseName", conn);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        public async Task<Warehouse?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            await using var cmd = new MySqlCommand("SELECT * FROM Warehouse WHERE WarehouseId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            return await rdr.ReadAsync(ct) ? Map(rdr) : null;
        }

        public async Task InsertAsync(Warehouse w, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = "INSERT INTO Warehouse (WarehouseId,WarehouseName,Location,Capacity) VALUES (@id,@name,@loc,@cap)";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id",   w.WarehouseId);
            cmd.Parameters.AddWithValue("@name", w.WarehouseName);
            cmd.Parameters.AddWithValue("@loc",  w.Location ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@cap",  w.Capacity.HasValue ? w.Capacity.Value : (object)DBNull.Value);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static Warehouse Map(MySqlDataReader r) => new()
        {
            WarehouseId   = r["WarehouseId"].ToString()!,
            WarehouseName = r["WarehouseName"].ToString()!,
            Location      = r["Location"] == DBNull.Value ? null : r["Location"].ToString(),
            Capacity      = r["Capacity"] == DBNull.Value ? null : Convert.ToDecimal(r["Capacity"])
        };
    }
}
