// Data/Seeds/InitialDataSeeder.cs
// Deliverable #8: Data import utility for initial product catalog.
// Seeds 5 categories, 3 warehouses, 3 suppliers, 20 products, and initial stock levels.
// Idempotent — checks existence before inserting. Safe to run on every startup.
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;

namespace InventoryManagementSystem.Data.Seeds
{
    public class InitialDataSeeder
    {
        private readonly IDbConnectionFactory _factory;
        private readonly ILogger<InitialDataSeeder> _logger;

        public InitialDataSeeder(IDbConnectionFactory factory, ILogger<InitialDataSeeder> logger)
        {
            _factory = factory;
            _logger  = logger;
        }

        public async Task SeedAsync(CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);

            // ── Guard: only seed if no data exists ──────────────────────────────
            var count = Convert.ToInt64(await new MySqlCommand(
                "SELECT COUNT(1) FROM Product", conn).ExecuteScalarAsync(ct));

            if (count > 0)
            {
                _logger.LogInformation("Seed skipped — {Count} products already exist.", count);
                return;
            }

            _logger.LogInformation("Seeding initial data...");

            await SeedCategoriesAsync(conn, ct);
            await SeedWarehousesAsync(conn, ct);
            await SeedSuppliersAsync(conn, ct);
            await SeedProductsAsync(conn, ct);
            await SeedStockLevelsAsync(conn, ct);

            _logger.LogInformation("Initial data seeded successfully.");
        }

        // ── 5 Product Categories ─────────────────────────────────────────────────
        private static async Task SeedCategoriesAsync(MySqlConnection conn, CancellationToken ct)
        {
            var categories = new[]
            {
                ("CAT-001", "Electronics",    "Electronic devices and components",      (string?)null),
                ("CAT-002", "Office Supplies","Stationery and office consumables",       null),
                ("CAT-003", "Furniture",      "Office and warehouse furniture",          null),
                ("CAT-004", "Peripherals",    "Computer peripherals and accessories",   "CAT-001"),
                ("CAT-005", "Consumables",    "Ink, toner and print consumables",       "CAT-002")
            };

            const string sql = @"INSERT IGNORE INTO ProductCategory
                (CategoryId, CategoryName, Description, ParentCategoryId)
                VALUES (@id, @name, @desc, @parent)";

            foreach (var (id, name, desc, parent) in categories)
            {
                await using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id",     id);
                cmd.Parameters.AddWithValue("@name",   name);
                cmd.Parameters.AddWithValue("@desc",   desc);
                cmd.Parameters.AddWithValue("@parent", parent ?? (object)DBNull.Value);
                await cmd.ExecuteNonQueryAsync(ct);
            }
        }

        // ── 3 Warehouses ─────────────────────────────────────────────────────────
        private static async Task SeedWarehousesAsync(MySqlConnection conn, CancellationToken ct)
        {
            var warehouses = new[]
            {
                ("WH-001", "Main Warehouse",   "Mumbai, Maharashtra",   5000.00m),
                ("WH-002", "North Hub",        "Delhi, NCR",            3000.00m),
                ("WH-003", "South Depot",      "Bengaluru, Karnataka",  2000.00m)
            };

            const string sql = @"INSERT IGNORE INTO Warehouse
                (WarehouseId, WarehouseName, Location, Capacity)
                VALUES (@id, @name, @loc, @cap)";

            foreach (var (id, name, loc, cap) in warehouses)
            {
                await using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id",   id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@loc",  loc);
                cmd.Parameters.AddWithValue("@cap",  cap);
                await cmd.ExecuteNonQueryAsync(ct);
            }
        }

        // ── 3 Suppliers ──────────────────────────────────────────────────────────
        private static async Task SeedSuppliersAsync(MySqlConnection conn, CancellationToken ct)
        {
            var suppliers = new[]
            {
                ("SUP-001", "TechSource India",   "procurement@techsource.in",   "+91-22-4567-8900", "www.techsource.in"),
                ("SUP-002", "OfficeWorld Pvt Ltd","orders@officeworld.co.in",    "+91-11-2345-6789", "www.officeworld.co.in"),
                ("SUP-003", "FurniPro Suppliers", "supply@furnipro.com",         "+91-80-3456-7890", "www.furnipro.com")
            };

            const string sql = @"INSERT IGNORE INTO Supplier
                (SupplierId, SupplierName, Email, Phone, Website)
                VALUES (@id, @name, @email, @phone, @web)";

            foreach (var (id, name, email, phone, web) in suppliers)
            {
                await using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id",    id);
                cmd.Parameters.AddWithValue("@name",  name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@web",   web);
                await cmd.ExecuteNonQueryAsync(ct);
            }
        }

        // ── 20 Products ──────────────────────────────────────────────────────────
        private static async Task SeedProductsAsync(MySqlConnection conn, CancellationToken ct)
        {
            var products = new[]
            {
                ("PRD-001","LAPTOP-PRO-15",  "Laptop Pro 15 inch",       "CAT-001","PCS",  45000m, 55000m),
                ("PRD-002","LAPTOP-AIR-13",  "Laptop Air 13 inch",       "CAT-001","PCS",  35000m, 42000m),
                ("PRD-003","MONITOR-24FHD",  "24 inch FHD Monitor",      "CAT-001","PCS",  12000m, 15000m),
                ("PRD-004","MONITOR-27QHD",  "27 inch QHD Monitor",      "CAT-001","PCS",  22000m, 28000m),
                ("PRD-005","KEYBOARD-MECH",  "Mechanical Keyboard",      "CAT-004","PCS",   2500m,  3500m),
                ("PRD-006","MOUSE-WIRELESS", "Wireless Ergonomic Mouse", "CAT-004","PCS",    800m,  1200m),
                ("PRD-007","WEBCAM-1080P",   "HD Webcam 1080P",          "CAT-004","PCS",   1800m,  2500m),
                ("PRD-008","HEADSET-USB",    "USB Noise-Cancelling Headset","CAT-004","PCS",3200m,  4500m),
                ("PRD-009","A4-PAPER-500",   "A4 Paper Ream 500 sheets", "CAT-002","REAM",   120m,   180m),
                ("PRD-010","PEN-BALLPOINT",  "Ballpoint Pen Box 12pk",   "CAT-002","BOX",    60m,    90m),
                ("PRD-011","STAPLER-HEAVY",  "Heavy Duty Stapler",       "CAT-002","PCS",   250m,   380m),
                ("PRD-012","FILE-FOLDER-A4", "A4 File Folders (Pack 10)","CAT-002","PACK",   90m,   140m),
                ("PRD-013","TONER-HP-BLK",   "HP Black Toner Cartridge", "CAT-005","PCS",  2200m,  3000m),
                ("PRD-014","TONER-HP-CLR",   "HP Color Toner Set",       "CAT-005","SET",  6500m,  8500m),
                ("PRD-015","INK-EPSON-SET",  "Epson Ink Cartridge Set",  "CAT-005","SET",   800m,  1200m),
                ("PRD-016","DESK-EXECUTIVE", "Executive Office Desk",    "CAT-003","PCS", 18000m, 25000m),
                ("PRD-017","CHAIR-ERGONOMIC","Ergonomic Office Chair",   "CAT-003","PCS", 12000m, 18000m),
                ("PRD-018","SHELF-METAL-5T", "5-Tier Metal Shelf Unit",  "CAT-003","PCS",  5500m,  7500m),
                ("PRD-019","HDD-EXT-1TB",    "External HDD 1TB",         "CAT-001","PCS",  4500m,  6000m),
                ("PRD-020","USB-HUB-7PORT",  "7-Port USB Hub",           "CAT-004","PCS",   600m,   900m)
            };

            const string sql = @"INSERT IGNORE INTO Product
                (ProductId, SKU, ProductName, Description, CategoryId,
                 UnitOfMeasure, Cost, ListPrice, IsActive)
                VALUES (@id, @sku, @name, @desc, @cat, @uom, @cost, @price, 1)";

            foreach (var (id, sku, name, cat, uom, cost, price) in products)
            {
                await using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id",    id);
                cmd.Parameters.AddWithValue("@sku",   sku);
                cmd.Parameters.AddWithValue("@name",  name);
                cmd.Parameters.AddWithValue("@desc",  name);   // description = name for seed data
                cmd.Parameters.AddWithValue("@cat",   cat);
                cmd.Parameters.AddWithValue("@uom",   uom);
                cmd.Parameters.AddWithValue("@cost",  cost);
                cmd.Parameters.AddWithValue("@price", price);
                await cmd.ExecuteNonQueryAsync(ct);
            }
        }

        // ── Initial Stock Levels (each product in Main Warehouse) ────────────────
        private static async Task SeedStockLevelsAsync(MySqlConnection conn, CancellationToken ct)
        {
            // (ProductId, QtyOnHand, ReorderLevel, SafetyStock)
            var levels = new[]
            {
                ("PRD-001", 50m,  10m, 5m),
                ("PRD-002", 35m,   8m, 3m),
                ("PRD-003", 80m,  15m, 8m),
                ("PRD-004", 40m,  10m, 5m),
                ("PRD-005", 120m, 20m, 10m),
                ("PRD-006", 200m, 30m, 15m),
                ("PRD-007", 60m,  10m, 5m),
                ("PRD-008", 45m,   8m, 4m),
                ("PRD-009", 500m, 100m, 50m),
                ("PRD-010", 300m, 50m, 25m),
                ("PRD-011", 80m,  15m, 8m),
                ("PRD-012", 150m, 30m, 15m),
                ("PRD-013", 90m,  20m, 10m),
                ("PRD-014", 30m,   5m, 2m),
                ("PRD-015", 60m,  10m, 5m),
                ("PRD-016", 20m,   3m, 1m),
                ("PRD-017", 25m,   5m, 2m),
                ("PRD-018", 15m,   3m, 1m),
                ("PRD-019", 70m,  15m, 8m),
                ("PRD-020", 150m, 25m, 10m)
            };

            const string sql = @"INSERT IGNORE INTO StockLevel
                (StockLevelId, ProductId, WarehouseId, QuantityOnHand, ReorderLevel, SafetyStock, ReservedQuantity)
                VALUES (@slid, @pid, 'WH-001', @qty, @rl, @ss, 0)";

            foreach (var (pid, qty, rl, ss) in levels)
            {
                await using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@slid", $"SL-{pid.Replace("PRD-", "")}");
                cmd.Parameters.AddWithValue("@pid",  pid);
                cmd.Parameters.AddWithValue("@qty",  qty);
                cmd.Parameters.AddWithValue("@rl",   rl);
                cmd.Parameters.AddWithValue("@ss",   ss);
                await cmd.ExecuteNonQueryAsync(ct);
            }
        }
    }
}
