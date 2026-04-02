// tests/InventoryManagementSystem.Tests/Handlers/StockInCommandHandlerTests.cs
// TDD: Tests for StockIn handler — verifies UoW transaction, stock upsert, and transaction log
// Uses Moq to mock IUnitOfWork, IStockLevelRepository, IStockTransactionRepository
using Moq;
using Xunit;
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Inventory.Commands.StockIn;
using InventoryManagementSystem.Features.Inventory.Repository;

namespace InventoryManagementSystem.Tests.Handlers
{
    public class StockInCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork>                  _mockUow;
        private readonly Mock<IStockLevelRepository>        _mockStockRepo;
        private readonly Mock<IStockTransactionRepository>  _mockTxnRepo;
        private readonly StockInCommandHandler              _handler;

        public StockInCommandHandlerTests()
        {
            _mockUow       = new Mock<IUnitOfWork>();
            _mockStockRepo = new Mock<IStockLevelRepository>();
            _mockTxnRepo   = new Mock<IStockTransactionRepository>();

            // Setup the UoW connection/transaction stubs
            _mockUow.Setup(u => u.Connection)
                    .Returns((MySqlConnection)null!);
            _mockUow.Setup(u => u.Transaction)
                    .Returns((MySqlTransaction)null!);
            _mockUow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.RollbackAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            // UpsertAsync and InsertAsync return successfully by default
            _mockStockRepo.Setup(r => r.UpsertAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(),
                It.IsAny<MySqlConnection>(), It.IsAny<MySqlTransaction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockTxnRepo.Setup(r => r.InsertAsync(
                It.IsAny<StockTransaction>(),
                It.IsAny<MySqlConnection>(), It.IsAny<MySqlTransaction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _handler = new StockInCommandHandler(_mockUow.Object, _mockStockRepo.Object, _mockTxnRepo.Object);
        }

        [Fact]
        public async Task StockIn_ValidCommand_ShouldReturnSuccess()
        {
            var cmd = new StockInCommand("PRD-001", "WH-001", 50);

            var result = await _handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Contains("50", result.Value);
        }

        [Fact]
        public async Task StockIn_ValidCommand_ShouldCallBeginTransaction()
        {
            var cmd = new StockInCommand("PRD-001", "WH-001", 10);

            await _handler.Handle(cmd, CancellationToken.None);

            _mockUow.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StockIn_ValidCommand_ShouldCallUpsertStock()
        {
            var cmd = new StockInCommand("PRD-002", "WH-001", 25);

            await _handler.Handle(cmd, CancellationToken.None);

            _mockStockRepo.Verify(r => r.UpsertAsync(
                "PRD-002", "WH-001", 25,
                It.IsAny<MySqlConnection>(), It.IsAny<MySqlTransaction>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task StockIn_ValidCommand_ShouldLogTransaction()
        {
            var cmd = new StockInCommand("PRD-003", "WH-002", 100);

            await _handler.Handle(cmd, CancellationToken.None);

            _mockTxnRepo.Verify(r => r.InsertAsync(
                It.Is<StockTransaction>(t =>
                    t.ProductId == "PRD-003" &&
                    t.WarehouseId == "WH-002" &&
                    t.Quantity == 100),
                It.IsAny<MySqlConnection>(), It.IsAny<MySqlTransaction>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task StockIn_ValidCommand_ShouldCommitTransaction()
        {
            var cmd = new StockInCommand("PRD-001", "WH-001", 5);

            await _handler.Handle(cmd, CancellationToken.None);

            _mockUow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StockIn_WhenUpsertThrows_ShouldRollback()
        {
            _mockStockRepo.Setup(r => r.UpsertAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(),
                It.IsAny<MySqlConnection>(), It.IsAny<MySqlTransaction>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            var cmd = new StockInCommand("PRD-001", "WH-001", 10);

            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(cmd, CancellationToken.None));

            _mockUow.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
