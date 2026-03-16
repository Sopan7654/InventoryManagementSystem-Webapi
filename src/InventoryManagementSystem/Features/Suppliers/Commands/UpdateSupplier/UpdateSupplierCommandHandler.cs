// Features/Suppliers/Commands/UpdateSupplier/UpdateSupplierCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Suppliers.Repository;
namespace InventoryManagementSystem.Features.Suppliers.Commands.UpdateSupplier
{
    public sealed class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result<bool>>
    {
        private readonly ISupplierRepository _repo;
        public UpdateSupplierCommandHandler(ISupplierRepository repo) => _repo = repo;
        public async Task<Result<bool>> Handle(UpdateSupplierCommand cmd, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(cmd.SupplierId, ct)
                ?? throw new NotFoundException(nameof(Supplier), cmd.SupplierId);
            existing.SupplierName = cmd.SupplierName;
            existing.Email        = cmd.Email;
            existing.Phone        = cmd.Phone;
            existing.Website      = cmd.Website;
            return Result<bool>.Success(await _repo.UpdateAsync(existing, ct));
        }
    }
}
