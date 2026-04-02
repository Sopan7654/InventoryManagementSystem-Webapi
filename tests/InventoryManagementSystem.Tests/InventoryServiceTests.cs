// InventoryServiceTests.cs — Pure domain model unit tests (no live DB required)
// Namespaces fixed from old console app (InventoryManagementSystem.Models)
// to current WebAPI structure (Domain.Entities + Domain.Enumerations)
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enumerations;
using Xunit;

namespace InventoryManagementSystem.Tests
{
    // ============================================================
    // Unit Tests — Inventory Management System
    // 14 test cases covering model logic, computed properties,
    // and business rules
    // ============================================================

    // ============================================================
    // TEST CLASS 1: StockLevel — Computed Properties
    // ============================================================
    public class StockLevelModelTests
    {
        [Fact]
        public void AvailableQuantity_ShouldBeOnHandMinusReserved()
        {
            var sl = new StockLevel { QuantityOnHand = 100, ReservedQuantity = 15 };
            Assert.Equal(85, sl.AvailableQuantity);
        }

        [Fact]
        public void AvailableQuantity_ShouldBeZero_WhenAllReserved()
        {
            var sl = new StockLevel { QuantityOnHand = 50, ReservedQuantity = 50 };
            Assert.Equal(0, sl.AvailableQuantity);
        }

        [Fact]
        public void IsLowStock_ShouldBeTrue_WhenQtyEqualsReorderLevel()
        {
            var sl = new StockLevel { QuantityOnHand = 20, ReorderLevel = 20 };
            Assert.True(sl.IsLowStock);
        }

        [Fact]
        public void IsLowStock_ShouldBeTrue_WhenQtyBelowReorderLevel()
        {
            var sl = new StockLevel { QuantityOnHand = 5, ReorderLevel = 20 };
            Assert.True(sl.IsLowStock);
        }

        [Fact]
        public void IsLowStock_ShouldBeFalse_WhenQtyAboveReorderLevel()
        {
            var sl = new StockLevel { QuantityOnHand = 50, ReorderLevel = 20 };
            Assert.False(sl.IsLowStock);
        }
    }

    // ============================================================
    // TEST CLASS 2: Batch — Expiry Status
    // Convention: underscore format matches Batch.ExpiryStatus property
    // ============================================================
    public class BatchModelTests
    {
        [Fact]
        public void ExpiryStatus_ShouldReturnNoExpiry_WhenNoDate()
        {
            var b = new Batch { ExpiryDate = null };
            Assert.Equal("NO_EXPIRY", b.ExpiryStatus);
        }

        [Fact]
        public void ExpiryStatus_ShouldReturnExpired_WhenDateIsPast()
        {
            var b = new Batch { ExpiryDate = DateTime.Today.AddDays(-1) };
            Assert.Equal("EXPIRED", b.ExpiryStatus);
        }

        [Fact]
        public void ExpiryStatus_ShouldReturnExpiringSoon_Within30Days()
        {
            var b = new Batch { ExpiryDate = DateTime.Today.AddDays(10) };
            Assert.Equal("EXPIRING_SOON", b.ExpiryStatus);
        }

        [Fact]
        public void ExpiryStatus_ShouldReturnExpiringSoon_ExactlyToday()
        {
            var b = new Batch { ExpiryDate = DateTime.Today };
            Assert.Equal("EXPIRING_SOON", b.ExpiryStatus);
        }

        [Fact]
        public void ExpiryStatus_ShouldReturnOK_WhenMoreThan30DaysAway()
        {
            var b = new Batch { ExpiryDate = DateTime.Today.AddDays(60) };
            Assert.Equal("OK", b.ExpiryStatus);
        }
    }

    // ============================================================
    // TEST CLASS 3: Product — Defaults
    // ============================================================
    public class ProductModelTests
    {
        [Fact]
        public void Product_DefaultIsActive_ShouldBeTrue()
        {
            var p = new Product();
            Assert.True(p.IsActive);
        }

        [Fact]
        public void Product_DefaultUOM_ShouldBePCS()
        {
            var p = new Product();
            Assert.Equal("PCS", p.UnitOfMeasure);
        }
    }

    // ============================================================
    // TEST CLASS 4: PurchaseOrderItem — LineTotal
    // PurchaseOrderItem is defined in Domain.Entities (inside PurchaseOrder.cs)
    // ============================================================
    public class PurchaseOrderItemTests
    {
        [Fact]
        public void LineTotal_ShouldBeQuantityTimesUnitPrice()
        {
            var item = new PurchaseOrderItem { QuantityOrdered = 10, UnitPrice = 250.50m };
            Assert.Equal(2505.00m, item.LineTotal);
        }

        [Fact]
        public void LineTotal_ShouldBeZero_WhenQtyIsZero()
        {
            var item = new PurchaseOrderItem { QuantityOrdered = 0, UnitPrice = 100 };
            Assert.Equal(0, item.LineTotal);
        }
    }

    // ============================================================
    // TEST CLASS 5: TransactionType constants (Domain.Enumerations)
    // Class name: TransactionType (not TransactionTypes — old console app name)
    // ============================================================
    public class TransactionTypeTests
    {
        [Fact]
        public void Purchase_ShouldEqualPURCHASE()
        {
            Assert.Equal("PURCHASE", TransactionType.Purchase);
        }

        [Fact]
        public void Sale_ShouldEqualSALE()
        {
            Assert.Equal("SALE", TransactionType.Sale);
        }

        [Fact]
        public void TransferIn_ShouldEqualTRANSFER_IN()
        {
            Assert.Equal("TRANSFER_IN", TransactionType.TransferIn);
        }

        [Fact]
        public void Hold_ShouldEqualHOLD()
        {
            Assert.Equal("HOLD", TransactionType.Hold);
        }
    }
}
