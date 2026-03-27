// ============================================================
// FILE: tests/InventoryManagement.UnitTests/Application/Features/Products/CreateProductCommandHandlerTests.cs
// ============================================================
using AutoMapper;
using FluentAssertions;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Events;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InventoryManagement.UnitTests.Application.Features.Products
{
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<IEventPublisher> _mockEvents;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockCache = new Mock<ICacheService>();
            _mockEvents = new Mock<IEventPublisher>();
            var mockLogger = new Mock<ILogger<CreateProductCommandHandler>>();

            _handler = new CreateProductCommandHandler(
                _mockUow.Object, _mockMapper.Object, mockLogger.Object, _mockCache.Object, _mockEvents.Object);
        }

        [Fact]
        public void CreateProduct_WithDuplicateSKU_ReturnsConflictResult()
        {
            // Arrange
            var dto = new CreateProductDto { SKU = "DUP123", ProductName = "Test" };
            var command = new CreateProductCommand(dto);
            
            _mockUow.Setup(u => u.Products.SKUExistsAsync("DUP123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = _handler.Handle(command, CancellationToken.None).Result;

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(Shared.ResultError.Conflict);
            result.Error.Should().Contain("already exists");
            
            _mockUow.Verify(u => u.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void CreateProduct_WithValidData_CreatesProductAndPublishesEvent()
        {
            // Arrange
            var dto = new CreateProductDto { SKU = "NEW123", ProductName = "New Product" };
            var command = new CreateProductCommand(dto);
            
            _mockUow.Setup(u => u.Products.SKUExistsAsync("NEW123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var product = new Product { ProductId = Guid.NewGuid().ToString(), SKU = "NEW123", ProductName = "New Product" };
            _mockMapper.Setup(m => m.Map<Product>(dto)).Returns(product);
            
            var responseDto = new ProductResponseDto { ProductId = product.ProductId, SKU = product.SKU };
            _mockMapper.Setup(m => m.Map<ProductResponseDto>(product)).Returns(responseDto);

            // Act
            var result = _handler.Handle(command, CancellationToken.None).Result;

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.SKU.Should().Be("NEW123");

            _mockUow.Verify(u => u.Products.AddAsync(product, It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockCache.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockEvents.Verify(e => e.PublishAsync(It.IsAny<ProductCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
