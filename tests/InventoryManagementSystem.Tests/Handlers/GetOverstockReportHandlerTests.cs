// tests/InventoryManagementSystem.Tests/Handlers/GetOverstockReportHandlerTests.cs
// TDD: handler tests for the Overstock Report query.
using Moq;
using Xunit;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Inventory.Repository;
using InventoryManagementSystem.Features.Reports.Queries.GetOverstockReport;

namespace InventoryManagementSystem.Tests.Handlers
{
    public class GetOverstockReportHandlerTests
    {
        private readonly Mock<IStockLevelRepository>       _mockRepo;
        private readonly GetOverstockReportQueryHandler    _handler;

        public GetOverstockReportHandlerTests()
        {
            _mockRepo = new Mock<IStockLevelRepository>();
            _handler  = new GetOverstockReportQueryHandler(_mockRepo.Object);
        }

        private static List<StockLevel> SampleLevels() => new()
        {
            new StockLevel { ProductId = "PRD-001", ProductName = "Laptop",
                             WarehouseName = "Main WH", QuantityOnHand = 300, ReorderLevel = 10 },

            new StockLevel { ProductId = "PRD-002", ProductName = "Mouse",
                             WarehouseName = "Main WH", QuantityOnHand = 50, ReorderLevel = 10 },

            new StockLevel { ProductId = "PRD-003", ProductName = "Paper",
                             WarehouseName = "Main WH", QuantityOnHand = 5, ReorderLevel = 100 },
        };

        [Fact]
        public async Task GetOverstock_DefaultThreshold_ShouldFlagAbove3xReorderLevel()
        {
            // PRD-001: 300 > 10 × 3 = 30 → overstocked ✓
            // PRD-002: 50 > 10 × 3 = 30 → overstocked ✓
            // PRD-003: 5 < 100 × 3 = 300 → NOT overstocked ✗
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(SampleLevels());

            var result = await _handler.Handle(new GetOverstockReportQuery(3m), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Count);
            Assert.Contains(result.Value, x => x.ProductId == "PRD-001");
            Assert.Contains(result.Value, x => x.ProductId == "PRD-002");
        }

        [Fact]
        public async Task GetOverstock_HighThreshold_ShouldReturnEmpty()
        {
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(SampleLevels());

            // Use 1000x threshold — nothing should qualify
            var result = await _handler.Handle(new GetOverstockReportQuery(1000m), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetOverstock_ZeroReorderLevel_ShouldSkipProduct()
        {
            // Products with ReorderLevel = 0 should never be flagged (divide by zero guard)
            var levels = new List<StockLevel>
            {
                new StockLevel { ProductId = "PRD-Z", ProductName = "Zerostock",
                                 WarehouseName = "WH", QuantityOnHand = 999, ReorderLevel = 0 }
            };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(levels);

            var result = await _handler.Handle(new GetOverstockReportQuery(3m), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetOverstock_ShouldOrderByExcessQuantityDescending()
        {
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(SampleLevels());

            var result = await _handler.Handle(new GetOverstockReportQuery(3m), CancellationToken.None);

            var items = result.Value!;
            Assert.True(items[0].ExcessQuantity >= items[1].ExcessQuantity);
        }
    }
}
