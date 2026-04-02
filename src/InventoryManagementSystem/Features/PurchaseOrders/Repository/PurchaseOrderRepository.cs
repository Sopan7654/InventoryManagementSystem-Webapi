// Features/PurchaseOrders/Repository/PurchaseOrderRepository.cs
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.PurchaseOrders.Repository
{
    public sealed class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly IDbConnectionFactory _factory;
        public PurchaseOrderRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<List<PurchaseOrder>> GetAllAsync(CancellationToken ct = default)
        {
            var list = new List<PurchaseOrder>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT po.PurchaseOrderId,po.SupplierId,s.SupplierName,po.OrderDate,po.Status,
                                        COUNT(poi.POItemId) AS ItemCount,
                                        COALESCE(SUM(poi.QuantityOrdered*poi.UnitPrice),0) AS TotalAmount
                                 FROM PurchaseOrder po
                                 JOIN Supplier s ON s.SupplierId=po.SupplierId
                                 LEFT JOIN PurchaseOrderItem poi ON poi.PurchaseOrderId=po.PurchaseOrderId
                                 GROUP BY po.PurchaseOrderId,po.SupplierId,s.SupplierName,po.OrderDate,po.Status
                                 ORDER BY po.OrderDate DESC";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct)) list.Add(MapPO(rdr));
            return list;
        }

        public async Task<PurchaseOrder?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT po.PurchaseOrderId,po.SupplierId,s.SupplierName,po.OrderDate,po.Status,
                                        COUNT(poi.POItemId) AS ItemCount,
                                        COALESCE(SUM(poi.QuantityOrdered*poi.UnitPrice),0) AS TotalAmount
                                 FROM PurchaseOrder po
                                 JOIN Supplier s ON s.SupplierId=po.SupplierId
                                 LEFT JOIN PurchaseOrderItem poi ON poi.PurchaseOrderId=po.PurchaseOrderId
                                 WHERE po.PurchaseOrderId=@id
                                 GROUP BY po.PurchaseOrderId,po.SupplierId,s.SupplierName,po.OrderDate,po.Status";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            return await rdr.ReadAsync(ct) ? MapPO(rdr) : null;
        }

        public async Task<List<PurchaseOrderItem>> GetItemsByPOAsync(string poId, CancellationToken ct = default)
        {
            var list = new List<PurchaseOrderItem>();
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"SELECT poi.POItemId,poi.PurchaseOrderId,poi.ProductId,p.ProductName,poi.QuantityOrdered,poi.UnitPrice
                                 FROM PurchaseOrderItem poi
                                 JOIN Product p ON p.ProductId=poi.ProductId
                                 WHERE poi.PurchaseOrderId=@poid";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@poid", poId);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct))
                list.Add(new PurchaseOrderItem
                {
                    POItemId = rdr["POItemId"].ToString()!, PurchaseOrderId = rdr["PurchaseOrderId"].ToString()!,
                    ProductId = rdr["ProductId"].ToString()!, ProductName = rdr["ProductName"].ToString()!,
                    QuantityOrdered = Convert.ToDecimal(rdr["QuantityOrdered"]), UnitPrice = Convert.ToDecimal(rdr["UnitPrice"])
                });
            return list;
        }

        public async Task InsertPOAsync(PurchaseOrder po, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            await using var cmd = new MySqlCommand("INSERT INTO PurchaseOrder(PurchaseOrderId,SupplierId,OrderDate,Status) VALUES(@id,@sid,@date,@st)", conn);
            cmd.Parameters.AddWithValue("@id", po.PurchaseOrderId);
            cmd.Parameters.AddWithValue("@sid", po.SupplierId);
            cmd.Parameters.AddWithValue("@date", po.OrderDate.Date);
            cmd.Parameters.AddWithValue("@st", po.Status);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task InsertItemAsync(PurchaseOrderItem item, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            await using var cmd = new MySqlCommand("INSERT INTO PurchaseOrderItem(POItemId,PurchaseOrderId,ProductId,QuantityOrdered,UnitPrice) VALUES(@id,@poid,@pid,@qty,@price)", conn);
            cmd.Parameters.AddWithValue("@id", item.POItemId); cmd.Parameters.AddWithValue("@poid", item.PurchaseOrderId);
            cmd.Parameters.AddWithValue("@pid", item.ProductId); cmd.Parameters.AddWithValue("@qty", item.QuantityOrdered);
            cmd.Parameters.AddWithValue("@price", item.UnitPrice);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task<bool> UpdateStatusAsync(string poId, string status, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            await using var cmd = new MySqlCommand("UPDATE PurchaseOrder SET Status=@st WHERE PurchaseOrderId=@id", conn);
            cmd.Parameters.AddWithValue("@st", status); cmd.Parameters.AddWithValue("@id", poId);
            return await cmd.ExecuteNonQueryAsync(ct) > 0;
        }

        private static PurchaseOrder MapPO(System.Data.Common.DbDataReader r) => new()
        {
            PurchaseOrderId = r["PurchaseOrderId"].ToString()!, SupplierId = r["SupplierId"].ToString()!,
            SupplierName = r["SupplierName"].ToString()!, OrderDate = Convert.ToDateTime(r["OrderDate"]),
            Status = r["Status"].ToString()!, ItemCount = Convert.ToInt32(r["ItemCount"]),
            TotalAmount = Convert.ToDecimal(r["TotalAmount"])
        };
    }
}
