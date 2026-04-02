// Features/Inventory/Repository/StockLevelRepository.cs
using System.Data;
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Inventory.Repository
{
    public sealed class StockLevelRepository : IStockLevelRepository
    {
        private readonly IDbConnectionFactory _factory;
        public StockLevelRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<List<StockLevel>> GetAllAsync(CancellationToken ct = default)
        {
            var list = new List<StockLevel>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT sl.StockLevelId,sl.ProductId,p.ProductName,sl.WarehouseId,w.WarehouseName,
                                        sl.QuantityOnHand,sl.ReorderLevel,sl.SafetyStock,sl.ReservedQuantity
                                 FROM StockLevel sl
                                 JOIN Product p ON p.ProductId=sl.ProductId
                                 JOIN Warehouse w ON w.WarehouseId=sl.WarehouseId
                                 ORDER BY p.ProductName,w.WarehouseName";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        public async Task<StockLevel?> GetByProductAndWarehouseAsync(
            string productId, string warehouseId, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT sl.StockLevelId,sl.ProductId,p.ProductName,sl.WarehouseId,w.WarehouseName,
                                        sl.QuantityOnHand,sl.ReorderLevel,sl.SafetyStock,sl.ReservedQuantity
                                 FROM StockLevel sl
                                 JOIN Product p ON p.ProductId=sl.ProductId
                                 JOIN Warehouse w ON w.WarehouseId=sl.WarehouseId
                                 WHERE sl.ProductId=@pid AND sl.WarehouseId=@wid";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@wid", warehouseId);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            return await rdr.ReadAsync(ct) ? Map(rdr) : null;
        }

        public async Task<List<StockLevel>> GetLowStockAsync(CancellationToken ct = default)
        {
            var list = new List<StockLevel>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT sl.StockLevelId,sl.ProductId,p.ProductName,sl.WarehouseId,w.WarehouseName,
                                        sl.QuantityOnHand,sl.ReorderLevel,sl.SafetyStock,sl.ReservedQuantity
                                 FROM StockLevel sl
                                 JOIN Product p ON p.ProductId=sl.ProductId
                                 JOIN Warehouse w ON w.WarehouseId=sl.WarehouseId
                                 WHERE sl.QuantityOnHand <= sl.ReorderLevel AND p.IsActive=1
                                 ORDER BY (sl.ReorderLevel-sl.QuantityOnHand) DESC";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        /// <summary>Returns all StockLevel rows for a given product across all warehouses.</summary>
        public async Task<List<StockLevel>> GetByProductAsync(string productId, CancellationToken ct = default)
        {
            var list = new List<StockLevel>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT sl.StockLevelId,sl.ProductId,p.ProductName,sl.WarehouseId,w.WarehouseName,
                                        sl.QuantityOnHand,sl.ReorderLevel,sl.SafetyStock,sl.ReservedQuantity
                                 FROM StockLevel sl
                                 JOIN Product p ON p.ProductId=sl.ProductId
                                 JOIN Warehouse w ON w.WarehouseId=sl.WarehouseId
                                 WHERE sl.ProductId=@pid
                                 ORDER BY w.WarehouseName";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pid", productId);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        // Unit of Work overloads — share the caller's connection + transaction
        public async Task UpsertAsync(string productId, string warehouseId, decimal qtyDelta,
            MySqlConnection conn, MySqlTransaction txn, CancellationToken ct = default)
        {
            const string check = "SELECT COUNT(1) FROM StockLevel WHERE ProductId=@pid AND WarehouseId=@wid";
            await using var chkCmd = new MySqlCommand(check, conn, txn);
            chkCmd.Parameters.AddWithValue("@pid", productId);
            chkCmd.Parameters.AddWithValue("@wid", warehouseId);
            var exists = Convert.ToInt64(await chkCmd.ExecuteScalarAsync(ct)) > 0;

            if (exists)
            {
                await UpdateQuantityAsync(productId, warehouseId, qtyDelta, conn, txn, ct);
            }
            else
            {
                const string ins = @"INSERT INTO StockLevel
                    (StockLevelId,ProductId,WarehouseId,QuantityOnHand,ReorderLevel,SafetyStock,ReservedQuantity)
                    VALUES (@id,@pid,@wid,@qty,0,0,0)";
                await using var cmd = new MySqlCommand(ins, conn, txn);
                cmd.Parameters.AddWithValue("@id",  Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.Parameters.AddWithValue("@wid", warehouseId);
                cmd.Parameters.AddWithValue("@qty", qtyDelta);
                await cmd.ExecuteNonQueryAsync(ct);
            }
        }

        public async Task UpdateQuantityAsync(string productId, string warehouseId, decimal delta,
            MySqlConnection conn, MySqlTransaction txn, CancellationToken ct = default)
        {
            const string sql = "UPDATE StockLevel SET QuantityOnHand=QuantityOnHand+@delta WHERE ProductId=@pid AND WarehouseId=@wid";
            await using var cmd = new MySqlCommand(sql, conn, txn);
            cmd.Parameters.AddWithValue("@delta", delta);
            cmd.Parameters.AddWithValue("@pid",   productId);
            cmd.Parameters.AddWithValue("@wid",   warehouseId);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task UpdateReservedAsync(string productId, string warehouseId, decimal delta,
            MySqlConnection conn, MySqlTransaction txn, CancellationToken ct = default)
        {
            const string sql = "UPDATE StockLevel SET ReservedQuantity=ReservedQuantity+@delta WHERE ProductId=@pid AND WarehouseId=@wid";
            await using var cmd = new MySqlCommand(sql, conn, txn);
            cmd.Parameters.AddWithValue("@delta", delta);
            cmd.Parameters.AddWithValue("@pid",   productId);
            cmd.Parameters.AddWithValue("@wid",   warehouseId);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static StockLevel Map(System.Data.Common.DbDataReader r) => new()
        {
            StockLevelId     = r["StockLevelId"].ToString()!,
            ProductId        = r["ProductId"].ToString()!,
            ProductName      = r["ProductName"].ToString()!,
            WarehouseId      = r["WarehouseId"].ToString()!,
            WarehouseName    = r["WarehouseName"].ToString()!,
            QuantityOnHand   = Convert.ToDecimal(r["QuantityOnHand"]),
            ReorderLevel     = Convert.ToDecimal(r["ReorderLevel"]),
            SafetyStock      = Convert.ToDecimal(r["SafetyStock"]),
            ReservedQuantity = Convert.ToDecimal(r["ReservedQuantity"])
        };
    }
}
