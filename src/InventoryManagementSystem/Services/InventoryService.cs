// Services/InventoryService.cs
using InventoryManagementSystem.DataAccess;
using InventoryManagementSystem.Models;
using MySql.Data.MySqlClient;

namespace InventoryManagementSystem.Services
{
    public class InventoryService
    {
        private readonly StockLevelRepository _stockRepo;
        private readonly StockTransactionRepository _txnRepo;

        public InventoryService()
        {
            _stockRepo = new StockLevelRepository();
            _txnRepo = new StockTransactionRepository();
        }

        // Constructor for testing (dependency injection)
        public InventoryService(StockLevelRepository stockRepo, StockTransactionRepository txnRepo)
        {
            _stockRepo = stockRepo;
            _txnRepo = txnRepo;
        }

        // ════════════════════════════════════════════════════════════════
        //  PUBLIC OPERATIONS
        // ════════════════════════════════════════════════════════════════

        public (bool Success, string Message) StockIn(string productId, string warehouseId, decimal qty)
        {
            if (qty <= 0)
                return Fail("Quantity must be greater than zero.");

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var validationError = ValidateProductAndWarehouse(conn, productId, warehouseId);
            if (validationError != null)
                return Fail(validationError);

            using var txn = conn.BeginTransaction();
            try
            {
                UpsertStockLevel(conn, txn, productId, warehouseId, qty);
                InsertTransaction(conn, txn, productId, warehouseId, TransactionTypes.Purchase, qty);

                txn.Commit();
                return Ok($"Stock In successful. +{qty} units added.");
            }
            catch (Exception ex)
            {
                txn.Rollback();
                return Fail(ex.Message);
            }
        }

        public (bool Success, string Message) StockOut(string productId, string warehouseId, decimal qty)
        {
            if (qty <= 0)
                return Fail("Quantity must be greater than zero.");

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var validationError = ValidateProductAndWarehouse(conn, productId, warehouseId);
            if (validationError != null)
                return Fail(validationError);

            var stock = GetStockLevel(conn, productId, warehouseId);
            if (stock == null)
                return Fail("No stock record found for this product/warehouse combination.");
            if (stock.AvailableQuantity < qty)
                return Fail($"Insufficient stock. Available: {stock.AvailableQuantity:N2}, Requested: {qty:N2}");

            using var txn = conn.BeginTransaction();
            try
            {
                UpdateStockQuantity(conn, txn, productId, warehouseId, -qty);
                InsertTransaction(conn, txn, productId, warehouseId, TransactionTypes.Sale, qty);

                txn.Commit();
                return Ok($"Stock Out successful. -{qty} units removed.");
            }
            catch (Exception ex)
            {
                txn.Rollback();
                return Fail(ex.Message);
            }
        }

        public (bool Success, string Message) Transfer(string productId, string fromWarehouseId, string toWarehouseId, decimal qty)
        {
            if (qty <= 0)
                return Fail("Quantity must be greater than zero.");
            if (string.Equals(fromWarehouseId, toWarehouseId, StringComparison.OrdinalIgnoreCase))
                return Fail("Source and destination warehouses must be different.");

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var prodError = ValidateProductExists(conn, productId);
            if (prodError != null) return Fail(prodError);

            var fromError = ValidateWarehouseExists(conn, fromWarehouseId);
            if (fromError != null) return Fail($"Source warehouse error: {fromError}");

            var toError = ValidateWarehouseExists(conn, toWarehouseId);
            if (toError != null) return Fail($"Destination warehouse error: {toError}");

            var stock = GetStockLevel(conn, productId, fromWarehouseId);
            if (stock == null)
                return Fail("No stock in source warehouse for this product.");
            if (stock.AvailableQuantity < qty)
                return Fail($"Insufficient stock in source. Available: {stock.AvailableQuantity:N2}");

            string reference = $"TRF-{DateTime.Now:yyyyMMddHHmmss}";

            using var txn = conn.BeginTransaction();
            try
            {
                // Deduct from source
                UpdateStockQuantity(conn, txn, productId, fromWarehouseId, -qty);

                // Add to destination (create if first time)
                UpsertStockLevel(conn, txn, productId, toWarehouseId, qty);

                // Log both sides
                InsertTransaction(conn, txn, productId, fromWarehouseId, TransactionTypes.TransferOut, qty, reference);
                InsertTransaction(conn, txn, productId, toWarehouseId, TransactionTypes.TransferIn, qty, reference);

                txn.Commit();
                return Ok($"Transfer complete. {qty} units moved from {fromWarehouseId} → {toWarehouseId}. Ref: {reference}");
            }
            catch (Exception ex)
            {
                txn.Rollback();
                return Fail(ex.Message);
            }
        }

        public (bool Success, string Message) HoldStock(string productId, string warehouseId, decimal qty)
        {
            if (qty <= 0)
                return Fail("Quantity must be greater than zero.");

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var validationError = ValidateProductAndWarehouse(conn, productId, warehouseId);
            if (validationError != null)
                return Fail(validationError);

            var stock = GetStockLevel(conn, productId, warehouseId);
            if (stock == null)
                return Fail("No stock record found for this product/warehouse combination.");
            if (stock.AvailableQuantity < qty)
                return Fail($"Cannot hold {qty:N2} units. Available (unreserved): {stock.AvailableQuantity:N2}");

            using var txn = conn.BeginTransaction();
            try
            {
                UpdateReservedQuantity(conn, txn, productId, warehouseId, qty);
                InsertTransaction(conn, txn, productId, warehouseId, TransactionTypes.Hold, qty);

                txn.Commit();
                return Ok($"{qty} units reserved/held successfully.");
            }
            catch (Exception ex)
            {
                txn.Rollback();
                return Fail(ex.Message);
            }
        }

        public (bool Success, string Message) Adjustment(string productId, string warehouseId, decimal qty, string? reason)
        {
            if (qty == 0)
                return Fail("Adjustment quantity cannot be zero.");

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var validationError = ValidateProductAndWarehouse(conn, productId, warehouseId);
            if (validationError != null)
                return Fail(validationError);

            var stock = GetStockLevel(conn, productId, warehouseId);
            if (stock == null)
                return Fail("No stock record found for this product/warehouse combination.");

            // Prevent negative stock after adjustment
            decimal projected = stock.QuantityOnHand + qty;
            if (projected < 0)
                return Fail($"Adjustment would result in negative stock ({projected:N2}). Current on-hand: {stock.QuantityOnHand:N2}");

            string refText = string.IsNullOrWhiteSpace(reason) ? null! : reason.Trim();

            using var txn = conn.BeginTransaction();
            try
            {
                UpdateStockQuantity(conn, txn, productId, warehouseId, qty);
                InsertTransaction(conn, txn, productId, warehouseId, TransactionTypes.Adjustment, qty, refText);

                txn.Commit();
                string sign = qty >= 0 ? "+" : "";
                return Ok($"Adjustment applied: {sign}{qty} units. New on-hand: {projected:N2}");
            }
            catch (Exception ex)
            {
                txn.Rollback();
                return Fail(ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════════════
        //  PRIVATE HELPERS — Validation
        // ════════════════════════════════════════════════════════════════

        /// <summary>Validates both product and warehouse exist. Returns null if valid, or an error message.</summary>
        private static string? ValidateProductAndWarehouse(MySqlConnection conn, string productId, string warehouseId)
        {
            return ValidateProductExists(conn, productId)
                ?? ValidateWarehouseExists(conn, warehouseId);
        }

        private static string? ValidateProductExists(MySqlConnection conn, string productId)
        {
            const string sql = "SELECT COUNT(1) FROM Product WHERE ProductId = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", productId);
            long count = (long)cmd.ExecuteScalar();
            return count == 0 ? $"Product '{productId}' does not exist." : null;
        }

        private static string? ValidateWarehouseExists(MySqlConnection conn, string warehouseId)
        {
            const string sql = "SELECT COUNT(1) FROM Warehouse WHERE WarehouseId = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", warehouseId);
            long count = (long)cmd.ExecuteScalar();
            return count == 0 ? $"Warehouse '{warehouseId}' does not exist." : null;
        }

        // ════════════════════════════════════════════════════════════════
        //  PRIVATE HELPERS — Stock Level Operations (within transaction)
        // ════════════════════════════════════════════════════════════════

        /// <summary>Gets the current stock level for a product/warehouse on the given connection.</summary>
        private static StockLevel? GetStockLevel(MySqlConnection conn, string productId, string warehouseId)
        {
            const string sql = @"SELECT StockLevelId, ProductId, WarehouseId,
                                        QuantityOnHand, ReorderLevel, SafetyStock, ReservedQuantity
                                 FROM StockLevel
                                 WHERE ProductId = @pid AND WarehouseId = @wid";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@wid", warehouseId);
            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return null;

            return new StockLevel
            {
                StockLevelId = rdr["StockLevelId"].ToString()!,
                ProductId = rdr["ProductId"].ToString()!,
                WarehouseId = rdr["WarehouseId"].ToString()!,
                QuantityOnHand = Convert.ToDecimal(rdr["QuantityOnHand"]),
                ReorderLevel = Convert.ToDecimal(rdr["ReorderLevel"]),
                SafetyStock = Convert.ToDecimal(rdr["SafetyStock"]),
                ReservedQuantity = Convert.ToDecimal(rdr["ReservedQuantity"])
            };
        }

        /// <summary>If stock record exists, adds delta to QuantityOnHand. Otherwise inserts a new row.</summary>
        private static void UpsertStockLevel(MySqlConnection conn, MySqlTransaction txn,
            string productId, string warehouseId, decimal qtyDelta)
        {
            const string checkSql = "SELECT COUNT(1) FROM StockLevel WHERE ProductId = @pid AND WarehouseId = @wid";
            using (var chk = new MySqlCommand(checkSql, conn, txn))
            {
                chk.Parameters.AddWithValue("@pid", productId);
                chk.Parameters.AddWithValue("@wid", warehouseId);
                long exists = (long)chk.ExecuteScalar();

                if (exists > 0)
                {
                    UpdateStockQuantity(conn, txn, productId, warehouseId, qtyDelta);
                    return;
                }
            }

            const string ins = @"INSERT INTO StockLevel
                                    (StockLevelId, ProductId, WarehouseId, QuantityOnHand, ReorderLevel, SafetyStock, ReservedQuantity)
                                 VALUES (@id, @pid, @wid, @qty, 0, 0, 0)";
            using var cmd = new MySqlCommand(ins, conn, txn);
            cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@wid", warehouseId);
            cmd.Parameters.AddWithValue("@qty", qtyDelta);
            cmd.ExecuteNonQuery();
        }

        private static void UpdateStockQuantity(MySqlConnection conn, MySqlTransaction txn,
            string productId, string warehouseId, decimal delta)
        {
            const string sql = "UPDATE StockLevel SET QuantityOnHand = QuantityOnHand + @delta WHERE ProductId = @pid AND WarehouseId = @wid";
            using var cmd = new MySqlCommand(sql, conn, txn);
            cmd.Parameters.AddWithValue("@delta", delta);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@wid", warehouseId);
            cmd.ExecuteNonQuery();
        }

        private static void UpdateReservedQuantity(MySqlConnection conn, MySqlTransaction txn,
            string productId, string warehouseId, decimal delta)
        {
            const string sql = "UPDATE StockLevel SET ReservedQuantity = ReservedQuantity + @delta WHERE ProductId = @pid AND WarehouseId = @wid";
            using var cmd = new MySqlCommand(sql, conn, txn);
            cmd.Parameters.AddWithValue("@delta", delta);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@wid", warehouseId);
            cmd.ExecuteNonQuery();
        }

        // ════════════════════════════════════════════════════════════════
        //  PRIVATE HELPERS — Transaction Logging
        // ════════════════════════════════════════════════════════════════

        private static void InsertTransaction(MySqlConnection conn, MySqlTransaction txn,
            string productId, string warehouseId, string transactionType, decimal qty, string? reference = null)
        {
            const string sql = @"INSERT INTO StockTransaction
                                    (TransactionId, ProductId, WarehouseId, TransactionType, Quantity, TransactionDate, Reference)
                                 VALUES (@id, @pid, @wid, @type, @qty, NOW(), @ref)";
            using var cmd = new MySqlCommand(sql, conn, txn);
            cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@wid", warehouseId);
            cmd.Parameters.AddWithValue("@type", transactionType);
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@ref", (object?)reference ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        // ════════════════════════════════════════════════════════════════
        //  PRIVATE HELPERS — Result Shortcuts
        // ════════════════════════════════════════════════════════════════

        private static (bool, string) Ok(string msg) => (true, msg);
        private static (bool, string) Fail(string msg) => (false, msg);
    }
}
