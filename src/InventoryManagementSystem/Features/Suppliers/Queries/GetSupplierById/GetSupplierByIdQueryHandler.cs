// Features/Suppliers/Queries/GetSupplierById/GetSupplierByIdQueryHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Suppliers.Repository;
namespace InventoryManagementSystem.Features.Suppliers.Queries.GetSupplierById
{
    public sealed class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, Result<Supplier>>
    {
        private readonly ISupplierRepository _repo;
        public GetSupplierByIdQueryHandler(ISupplierRepository repo) => _repo = repo;
        public async Task<Result<Supplier>> Handle(GetSupplierByIdQuery req, CancellationToken ct)
        {
            var s = await _repo.GetByIdAsync(req.SupplierId, ct)
                ?? throw new NotFoundException(nameof(Supplier), req.SupplierId);
            return Result<Supplier>.Success(s);
        }
    }
}
