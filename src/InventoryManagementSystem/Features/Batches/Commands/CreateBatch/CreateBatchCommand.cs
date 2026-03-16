// Features/Batches/Commands/CreateBatch/CreateBatchCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Batches.Commands.CreateBatch
{
    public sealed record CreateBatchCommand(
        string ProductId, string WarehouseId, string BatchNumber,
        DateTime? ManufacturingDate, DateTime? ExpiryDate, decimal Quantity
    ) : IRequest<Result<string>>;
}
