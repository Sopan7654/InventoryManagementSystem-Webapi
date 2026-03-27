// ============================================================
// FILE: tests/InventoryManagement.UnitTests/Application/Features/Inventory/StockInCommandHandlerTests.cs
// ============================================================
using FluentAssertions;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.Inventory.Commands;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Events;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InventoryManagement.UnitTests.Application.Features.Inventory
{
    public class StockInCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<IEventPublisher> _mockEvents;
        private readonly StockInCommandHandler _handler;

        public StockInCommandHandlerTests()
        {
            _mockUow = new Mock<IUnitOfWork> { DefaultValue = DefaultValue.Mock };
            _mockCache = new Mock<ICacheService>();
            _mockEvents = new Mock<IEventPublisher>();
            var mockLogger = new Mock<ILogger<StockInCommandHandler>>();

            _handler = new StockInCommandHandler(
                _mockUow.Object, mockLogger.Object, _mockCache.Object, _mockEvents.Object);
        }

        [Fact]
        public async Task StockIn_ProductNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var dto = new StockInDto { ProductId = "BadId", WarehouseId = "W1", Quantity = 10 };
            _mockUow.Setup(u => u.Products.GetByIdAsync("BadId", default)).ReturnsAsync((Product?)null);

            // Act
            var result = await _handler.Handle(new StockInCommand(dto), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(Shared.ResultError.NotFound);
            result.Error.Should().Contain("Product");
        }

        [Fact]
        public async Task StockIn_NewStockLevel_CreatesStockLevelAndTransaction()
        {
            // Arrange
            var dto = new StockInDto { ProductId = "P1", WarehouseId = "W1", Quantity = 10, Reference = "PO-001" };
            
            _mockUow.Setup(u => u.Products.GetByIdAsync("P1", default)).ReturnsAsync(new Product());
            _mockUow.Setup(u => u.Warehouses.GetByIdAsync("W1", default)).ReturnsAsync(new Warehouse());
            _mockUow.Setup(u => u.StockLevels.GetByProductAndWarehouseAsync("P1", "W1", default)).ReturnsAsync((StockLevel?)null);

            // Act
            var result = await _handler.Handle(new StockInCommand(dto), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            _mockUow.Verify(u => u.StockLevels.AddAsync(It.Is<StockLevel>(s => s.QuantityOnHand == 10), default), Times.Once);
            _mockUow.Verify(u => u.StockTransactions.AddAsync(It.Is<StockTransaction>(t => 
                t.Quantity == 10 && t.TransactionType == Domain.Enums.TransactionType.PURCHASE), default), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(default), Times.Once);
            _mockEvents.Verify(e => e.PublishAsync(It.IsAny<StockLevelChangedEvent>(), default), Times.Once);
        }

        [Fact]
        public async Task StockIn_ExistingStockLevel_UpdatesQuantity()
        {
            // Arrange
            var dto = new StockInDto { ProductId = "P1", WarehouseId = "W1", Quantity = 5 };
            var existingStock = new StockLevel { QuantityOnHand = 20 };

            _mockUow.Setup(u => u.Products.GetByIdAsync("P1", default)).ReturnsAsync(new Product());
            _mockUow.Setup(u => u.Warehouses.GetByIdAsync("W1", default)).ReturnsAsync(new Warehouse());
            _mockUow.Setup(u => u.StockLevels.GetByProductAndWarehouseAsync("P1", "W1", default)).ReturnsAsync(existingStock);

            // Act
            var result = await _handler.Handle(new StockInCommand(dto), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingStock.QuantityOnHand.Should().Be(25);
            
            _mockUow.Verify(u => u.StockLevels.Update(existingStock), Times.Once);
            _mockUow.Verify(u => u.StockLevels.AddAsync(It.IsAny<StockLevel>(), default), Times.Never);
        }
    }
}
