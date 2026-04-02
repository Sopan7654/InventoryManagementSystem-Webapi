// Features/Inventory/Repository/StockTransactionRepository.cs
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Inventory.Repository
{
    public sealed class StockTransactionRepository : IStockTransactionRepository
    {
        private readonly IDbConnectionFactory _factory;
        public StockTransactionRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<List<StockTransaction>> GetAllAsync(int limit = 50, CancellationToken ct = default)
        {
            var list = new List<StockTransaction>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            var sql = $@"SELECT st.TransactionId,st.ProductId,p.ProductName,st.WarehouseId,w.WarehouseName,
                                st.TransactionType,st.Quantity,st.TransactionDate,st.Reference
                         FROM StockTransaction st
                         JOIN Product p ON p.ProductId=st.ProductId
                         JOIN Warehouse w ON w.WarehouseId=st.WarehouseId
                         ORDER BY st.TransactionDate DESC LIMIT {limit}";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        public async Task<List<StockTransaction>> GetByProductAsync(string productId, CancellationToken ct = default)
        {
            var list = new List<StockTransaction>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT st.TransactionId,st.ProductId,p.ProductName,st.WarehouseId,w.WarehouseName,
                                        st.TransactionType,st.Quantity,st.TransactionDate,st.Reference
                                 FROM StockTransaction st
                                 JOIN Product p ON p.ProductId=st.ProductId
                                 JOIN Warehouse w ON w.WarehouseId=st.WarehouseId
                                 WHERE st.ProductId=@pid ORDER BY st.TransactionDate DESC";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pid", productId);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        /// <summary>Overload: filter by warehouseId (nullable) and cap results with limit.</summary>
        public async Task<List<StockTransaction>> GetByProductAsync(
            string productId, string? warehouseId, int limit, CancellationToken ct = default)
        {
            var list = new List<StockTransaction>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            var warehouseFilter = warehouseId != null ? "AND st.WarehouseId=@wid" : "";
            var sql = $@"SELECT st.TransactionId,st.ProductId,p.ProductName,st.WarehouseId,w.WarehouseName,
                                st.TransactionType,st.Quantity,st.TransactionDate,st.Reference
                         FROM StockTransaction st
                         JOIN Product p ON p.ProductId=st.ProductId
                         JOIN Warehouse w ON w.WarehouseId=st.WarehouseId
                         WHERE st.ProductId=@pid {warehouseFilter}
                         ORDER BY st.TransactionDate ASC LIMIT {limit}";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pid", productId);
            if (warehouseId != null) cmd.Parameters.AddWithValue("@wid", warehouseId);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(Map(rdr));
            return list;
        }

        // Unit of Work overload — insert within caller's transaction
        public async Task InsertAsync(StockTransaction t,
            MySqlConnection conn, MySqlTransaction dbTxn, CancellationToken ct = default)
        {
            const string sql = @"INSERT INTO StockTransaction
                                    (TransactionId,ProductId,WarehouseId,TransactionType,Quantity,TransactionDate,Reference)
                                 VALUES (@id,@pid,@wid,@type,@qty,NOW(),@ref)";
            await using var cmd = new MySqlCommand(sql, conn, dbTxn);
            cmd.Parameters.AddWithValue("@id",   t.TransactionId);
            cmd.Parameters.AddWithValue("@pid",  t.ProductId);
            cmd.Parameters.AddWithValue("@wid",  t.WarehouseId);
            cmd.Parameters.AddWithValue("@type", t.TransactionType);
            cmd.Parameters.AddWithValue("@qty",  t.Quantity);
            cmd.Parameters.AddWithValue("@ref",  (object?)t.Reference ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static StockTransaction Map(System.Data.Common.DbDataReader r) => new()
        {
            TransactionId   = r["TransactionId"].ToString()!,
            ProductId       = r["ProductId"].ToString()!,
            ProductName     = r["ProductName"].ToString()!,
            WarehouseId     = r["WarehouseId"].ToString()!,
            WarehouseName   = r["WarehouseName"].ToString()!,
            TransactionType = r["TransactionType"].ToString()!,
            Quantity        = Convert.ToDecimal(r["Quantity"]),
            TransactionDate = Convert.ToDateTime(r["TransactionDate"]),
            Reference       = r["Reference"] == DBNull.Value ? null : r["Reference"].ToString()
        };
    }
}
