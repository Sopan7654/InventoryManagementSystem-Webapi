// ============================================================
// FILE: src/InventoryManagement.Shared/Constants.cs
// ============================================================
namespace InventoryManagement.Shared
{
    /// <summary>
    /// Application-wide constants — no magic strings allowed.
    /// </summary>
    public static class Constants
    {
        /// <summary>Cache key templates for Redis.</summary>
        public static class CacheKeys
        {
            public const string AllProducts = "products:all";
            public const string ProductById = "products:{0}";
            public const string ProductBySku = "products:sku:{0}";
            public const string AllSuppliers = "suppliers:all";
            public const string SupplierById = "suppliers:{0}";
            public const string AllWarehouses = "warehouses:all";
            public const string WarehouseById = "warehouses:{0}";
            public const string StockLevel = "stock:{0}:{1}";
            public const string LowStockReport = "reports:low-stock";
            public const string ExpiringBatches = "reports:expiring-batches:{0}";
            public const string AllCategories = "categories:all";
            public const string CategoryById = "categories:{0}";
            public const string TokenBlacklist = "token:blacklist:{0}";
        }

        /// <summary>Custom claim types for JWT tokens.</summary>
        public static class ClaimTypes
        {
            public const string Department = "department";
            public const string Experience = "experience";
            public const string WarehouseAccess = "warehouseAccess";
            public const string UserId = "userId";
        }

        /// <summary>Authorization policy names.</summary>
        public static class Policies
        {
            public const string AdminOnly = "AdminOnly";
            public const string CanManageProducts = "CanManageProducts";
            public const string CanApproveOrders = "CanApproveOrders";
            public const string CanViewReports = "CanViewReports";
            public const string CanManageUsers = "CanManageUsers";
            public const string SeniorStaffOnly = "SeniorStaffOnly";
            public const string WarehouseAccess = "WarehouseAccess";
        }

        /// <summary>Role name constants.</summary>
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string Operator = "Operator";
            public const string Viewer = "Viewer";
        }

        /// <summary>CORS policy names.</summary>
        public static class CorsPolicies
        {
            public const string AllowSpecificOrigins = "AllowSpecificOrigins";
            public const string Development = "DevelopmentPolicy";
            public const string Production = "ProductionPolicy";
        }

        /// <summary>Default application values.</summary>
        public static class Defaults
        {
            public const string ValuationMethod = "FIFO";
            public const string UnitOfMeasure = "PCS";
            public const int DefaultPageSize = 20;
            public const int MaxPageSize = 100;
        }
    }
}
