// ============================================================
// FILE: src/InventoryManagement.Application/Features/Products/Commands/ProductCommands.cs
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Products.Commands
{
    /// <summary>Command to create a new product.</summary>
    public record CreateProductCommand(CreateProductDto Dto) : IRequest<Result<ProductResponseDto>>;

    /// <summary>Command to update an existing product.</summary>
    public record UpdateProductCommand(string ProductId, UpdateProductDto Dto) : IRequest<Result<ProductResponseDto>>;

    /// <summary>Command to soft-delete (deactivate) a product.</summary>
    public record DeleteProductCommand(string ProductId) : IRequest<Result<bool>>;

    /// <summary>Command to toggle product active status.</summary>
    public record ToggleProductStatusCommand(string ProductId, bool IsActive) : IRequest<Result<bool>>;
}
