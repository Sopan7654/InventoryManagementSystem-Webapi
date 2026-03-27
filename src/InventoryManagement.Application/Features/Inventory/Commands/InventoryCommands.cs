// ============================================================
// FILE: src/InventoryManagement.Application/Features/Inventory/Commands/InventoryCommands.cs
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Inventory.Commands
{
    /// <summary>Command to receive stock into a warehouse.</summary>
    public record StockInCommand(StockInDto Dto) : IRequest<Result<string>>;

    /// <summary>Command to ship stock out of a warehouse.</summary>
    public record StockOutCommand(StockOutDto Dto) : IRequest<Result<string>>;

    /// <summary>Command to transfer stock between warehouses.</summary>
    public record TransferStockCommand(StockTransferDto Dto) : IRequest<Result<string>>;

    /// <summary>Command to place stock on hold/reservation.</summary>
    public record HoldStockCommand(HoldStockDto Dto) : IRequest<Result<string>>;

    /// <summary>Command to perform a stock adjustment.</summary>
    public record AdjustmentCommand(AdjustmentDto Dto) : IRequest<Result<string>>;
}
