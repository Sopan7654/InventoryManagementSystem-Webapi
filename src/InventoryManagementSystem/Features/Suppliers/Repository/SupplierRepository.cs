// Features/Suppliers/Repository/SupplierRepository.cs
using System.Data.Common;
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Suppliers.Repository
{
    public sealed class SupplierRepository : ISupplierRepository
    {
        private readonly IDbConnectionFactory _factory;
        public SupplierRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<List<Supplier>> GetAllAsync(CancellationToken ct = default)
        {
            var list = new List<Supplier>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            await using var cmd = new MySqlCommand("SELECT * FROM Supplier ORDER BY SupplierName", conn);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        public async Task<Supplier?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            await using var cmd = new MySqlCommand("SELECT * FROM Supplier WHERE SupplierId=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            return await rdr.ReadAsync(ct) ? Map(rdr) : null;
        }

        public async Task InsertAsync(Supplier s, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"INSERT INTO Supplier (SupplierId,SupplierName,Email,Phone,Website)
                                 VALUES (@id,@name,@email,@phone,@web)";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id",    s.SupplierId);
            cmd.Parameters.AddWithValue("@name",  s.SupplierName);
            cmd.Parameters.AddWithValue("@email", s.Email   ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", s.Phone   ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@web",   s.Website ?? (object)DBNull.Value);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task<bool> UpdateAsync(Supplier s, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"UPDATE Supplier SET SupplierName=@name,Email=@email,
                                 Phone=@phone,Website=@web WHERE SupplierId=@id";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name",  s.SupplierName);
            cmd.Parameters.AddWithValue("@email", s.Email   ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@phone", s.Phone   ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@web",   s.Website ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id",    s.SupplierId);
            return await cmd.ExecuteNonQueryAsync(ct) > 0;
        }

        private static Supplier Map(DbDataReader r) => new()
        {
            SupplierId   = r["SupplierId"].ToString()!,
            SupplierName = r["SupplierName"].ToString()!,
            Email        = r["Email"]   == DBNull.Value ? null : r["Email"].ToString(),
            Phone        = r["Phone"]   == DBNull.Value ? null : r["Phone"].ToString(),
            Website      = r["Website"] == DBNull.Value ? null : r["Website"].ToString()
        };
    }
}
