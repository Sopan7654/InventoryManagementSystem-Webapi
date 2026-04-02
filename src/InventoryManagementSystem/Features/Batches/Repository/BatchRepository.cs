// Features/Batches/Repository/BatchRepository.cs
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Batches.Repository
{
    public sealed class BatchRepository : IBatchRepository
    {
        private readonly IDbConnectionFactory _factory;
        public BatchRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<List<Batch>> GetAllAsync(CancellationToken ct = default)
        {
            var list = new List<Batch>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT b.BatchId,b.ProductId,p.ProductName,b.WarehouseId,w.WarehouseName,
                                        b.BatchNumber,b.ManufacturingDate,b.ExpiryDate,b.Quantity
                                 FROM Batch b JOIN Product p ON p.ProductId=b.ProductId JOIN Warehouse w ON w.WarehouseId=b.WarehouseId
                                 ORDER BY b.ExpiryDate,p.ProductName";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        public async Task<List<Batch>> GetExpiringSoonAsync(int days = 30, CancellationToken ct = default)
        {
            var list = new List<Batch>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT b.BatchId,b.ProductId,p.ProductName,b.WarehouseId,w.WarehouseName,
                                        b.BatchNumber,b.ManufacturingDate,b.ExpiryDate,b.Quantity
                                 FROM Batch b JOIN Product p ON p.ProductId=b.ProductId JOIN Warehouse w ON w.WarehouseId=b.WarehouseId
                                 WHERE b.ExpiryDate IS NOT NULL AND b.ExpiryDate >= CURDATE()
                                   AND DATEDIFF(b.ExpiryDate,CURDATE()) <= @days
                                 ORDER BY b.ExpiryDate";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@days", days);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        public async Task InsertAsync(Batch b, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = "INSERT INTO Batch(BatchId,ProductId,WarehouseId,BatchNumber,ManufacturingDate,ExpiryDate,Quantity) VALUES(@id,@pid,@wid,@num,@mfg,@exp,@qty)";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id",  b.BatchId);
            cmd.Parameters.AddWithValue("@pid", b.ProductId);
            cmd.Parameters.AddWithValue("@wid", b.WarehouseId);
            cmd.Parameters.AddWithValue("@num", b.BatchNumber);
            cmd.Parameters.AddWithValue("@mfg", b.ManufacturingDate.HasValue ? b.ManufacturingDate.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@exp", b.ExpiryDate.HasValue ? b.ExpiryDate.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@qty", b.Quantity);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static Batch Map(System.Data.Common.DbDataReader r) => new()
        {
            BatchId           = r["BatchId"].ToString()!,
            ProductId         = r["ProductId"].ToString()!,
            ProductName       = r["ProductName"].ToString()!,
            WarehouseId       = r["WarehouseId"].ToString()!,
            WarehouseName     = r["WarehouseName"].ToString()!,
            BatchNumber       = r["BatchNumber"].ToString()!,
            ManufacturingDate = r["ManufacturingDate"] == DBNull.Value ? null : Convert.ToDateTime(r["ManufacturingDate"]),
            ExpiryDate        = r["ExpiryDate"]        == DBNull.Value ? null : Convert.ToDateTime(r["ExpiryDate"]),
            Quantity          = Convert.ToDecimal(r["Quantity"])
        };
    }
}
