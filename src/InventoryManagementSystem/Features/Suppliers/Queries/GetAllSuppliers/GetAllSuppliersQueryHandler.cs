// Features/Suppliers/Queries/GetAllSuppliers/GetAllSuppliersQueryHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Suppliers.Repository;
namespace InventoryManagementSystem.Features.Suppliers.Queries.GetAllSuppliers
{
    public sealed class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, Result<List<Supplier>>>
    {
        private readonly ISupplierRepository _repo;
        public GetAllSuppliersQueryHandler(ISupplierRepository repo) => _repo = repo;
        public async Task<Result<List<Supplier>>> Handle(GetAllSuppliersQuery req, CancellationToken ct)
            => Result<List<Supplier>>.Success(await _repo.GetAllAsync(ct));
    }
}
