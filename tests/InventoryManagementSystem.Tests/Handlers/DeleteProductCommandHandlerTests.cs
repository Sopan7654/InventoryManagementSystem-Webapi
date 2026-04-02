// tests/InventoryManagementSystem.Tests/Handlers/DeleteProductCommandHandlerTests.cs
// TDD: handler-level tests for soft-delete product command.
using Moq;
using Xunit;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Products.Commands.DeleteProduct;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Tests.Handlers
{
    public class DeleteProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository>      _mockRepo;
        private readonly DeleteProductCommandHandler   _handler;

        public DeleteProductCommandHandlerTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _handler  = new DeleteProductCommandHandler(_mockRepo.Object);
        }

        private static Product ActiveProduct(string id = "PRD-001") => new()
        {
            ProductId   = id,
            SKU         = "SKU-001",
            ProductName = "Test Laptop",
            IsActive    = true
        };

        [Fact]
        public async Task Delete_ActiveProduct_ShouldSoftDeleteIt()
        {
            var product = ActiveProduct();
            _mockRepo.Setup(r => r.GetByIdAsync("PRD-001", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(product);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

            var result = await _handler.Handle(new DeleteProductCommand("PRD-001"), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Contains("deactivated", result.Value, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Delete_ActiveProduct_ShouldSetIsActiveFalse()
        {
            Product? captured = null;
            var product = ActiveProduct();

            _mockRepo.Setup(r => r.GetByIdAsync("PRD-001", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(product);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                     .Callback<Product, CancellationToken>((p, _) => captured = p)
                     .ReturnsAsync(true);

            await _handler.Handle(new DeleteProductCommand("PRD-001"), CancellationToken.None);

            Assert.NotNull(captured);
            Assert.False(captured!.IsActive);
        }

        [Fact]
        public async Task Delete_NonExistentProduct_ShouldThrowNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(()
                => _handler.Handle(new DeleteProductCommand("MISSING"), CancellationToken.None));
        }

        [Fact]
        public async Task Delete_AlreadyInactiveProduct_ShouldReturnAlreadyInactiveMessage()
        {
            var product = ActiveProduct();
            product.IsActive = false;

            _mockRepo.Setup(r => r.GetByIdAsync("PRD-001", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(product);

            var result = await _handler.Handle(new DeleteProductCommand("PRD-001"), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Contains("already inactive", result.Value, StringComparison.OrdinalIgnoreCase);

            // UpdateAsync should NOT be called for already-inactive products
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
