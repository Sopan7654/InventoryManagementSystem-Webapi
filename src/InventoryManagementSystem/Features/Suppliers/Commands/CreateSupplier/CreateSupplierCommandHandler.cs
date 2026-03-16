// Features/Suppliers/Commands/CreateSupplier/CreateSupplierCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Suppliers.Repository;
namespace InventoryManagementSystem.Features.Suppliers.Commands.CreateSupplier
{
    public sealed class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<string>>
    {
        private readonly ISupplierRepository _repo;
        public CreateSupplierCommandHandler(ISupplierRepository repo) => _repo = repo;
        public async Task<Result<string>> Handle(CreateSupplierCommand cmd, CancellationToken ct)
        {
            var supplier = new Supplier
            {
                SupplierId   = Guid.NewGuid().ToString(),
                SupplierName = cmd.SupplierName,
                Email        = cmd.Email,
                Phone        = cmd.Phone,
                Website      = cmd.Website
            };
            await _repo.InsertAsync(supplier, ct);
            return Result<string>.Success(supplier.SupplierId);
        }
    }
}
