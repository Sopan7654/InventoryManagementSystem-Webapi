// ============================================================
// FILE: src/InventoryManagement.Application/Features/Suppliers/SupplierFeatures.cs
// ============================================================
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Suppliers
{
    // ── Commands ─────────────────────────────────────────────────
    public record CreateSupplierCommand(CreateSupplierDto Dto) : IRequest<Result<SupplierResponseDto>>;
    public record UpdateSupplierCommand(string SupplierId, UpdateSupplierDto Dto) : IRequest<Result<SupplierResponseDto>>;
    public record DeleteSupplierCommand(string SupplierId) : IRequest<Result<bool>>;

    // ── Queries ──────────────────────────────────────────────────
    public record GetAllSuppliersQuery(PaginationParams Pagination) : IRequest<Result<PaginatedResponse<SupplierResponseDto>>>, ICacheable
    {
        public string CacheKey => $"{Constants.CacheKeys.AllSuppliers}:{Pagination.PageNumber}:{Pagination.PageSize}";
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(10);
    }
    public record GetSupplierByIdQuery(string SupplierId) : IRequest<Result<SupplierResponseDto>>;

    // ── Command Handlers ─────────────────────────────────────────
    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<SupplierResponseDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly ILogger<CreateSupplierCommandHandler> _logger;

        public CreateSupplierCommandHandler(IUnitOfWork uow, IMapper mapper, ICacheService cache, ILogger<CreateSupplierCommandHandler> logger)
        { _uow = uow; _mapper = mapper; _cache = cache; _logger = logger; }

        public async Task<Result<SupplierResponseDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            Supplier supplier = _mapper.Map<Supplier>(request.Dto);
            await _uow.Suppliers.AddAsync(supplier, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByPatternAsync("suppliers:*", cancellationToken);
            _logger.LogInformation("Supplier created: {SupplierId}", supplier.SupplierId);
            return Result<SupplierResponseDto>.Success(_mapper.Map<SupplierResponseDto>(supplier));
        }
    }

    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result<SupplierResponseDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public UpdateSupplierCommandHandler(IUnitOfWork uow, IMapper mapper, ICacheService cache)
        { _uow = uow; _mapper = mapper; _cache = cache; }

        public async Task<Result<SupplierResponseDto>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            Supplier? supplier = await _uow.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
            if (supplier == null) return Result<SupplierResponseDto>.NotFound($"Supplier '{request.SupplierId}' not found.");
            _mapper.Map(request.Dto, supplier);
            _uow.Suppliers.Update(supplier);
            await _uow.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByPatternAsync("suppliers:*", cancellationToken);
            return Result<SupplierResponseDto>.Success(_mapper.Map<SupplierResponseDto>(supplier));
        }
    }

    public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICacheService _cache;
        private readonly ILogger<DeleteSupplierCommandHandler> _logger;

        public DeleteSupplierCommandHandler(IUnitOfWork uow, ICacheService cache, ILogger<DeleteSupplierCommandHandler> logger)
        { _uow = uow; _cache = cache; _logger = logger; }

        public async Task<Result<bool>> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
        {
            Supplier? supplier = await _uow.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
            if (supplier == null) return Result<bool>.NotFound($"Supplier '{request.SupplierId}' not found.");
            _uow.Suppliers.Delete(supplier);
            await _uow.SaveChangesAsync(cancellationToken);
            await _cache.RemoveByPatternAsync("suppliers:*", cancellationToken);
            _logger.LogInformation("Supplier deleted: {SupplierId}", request.SupplierId);
            return Result<bool>.Success(true);
        }
    }

    // ── Query Handlers ──────────────────────────────────────────
    public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, Result<PaginatedResponse<SupplierResponseDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetAllSuppliersQueryHandler(IUnitOfWork uow, IMapper mapper)
        { _uow = uow; _mapper = mapper; }

        public async Task<Result<PaginatedResponse<SupplierResponseDto>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<Supplier> items, int total) = await _uow.Suppliers.GetPagedAsync(
                request.Pagination.PageNumber, request.Pagination.PageSize, cancellationToken: cancellationToken);
            IEnumerable<SupplierResponseDto> dtos = _mapper.Map<IEnumerable<SupplierResponseDto>>(items);
            return Result<PaginatedResponse<SupplierResponseDto>>.Success(
                new PaginatedResponse<SupplierResponseDto>(dtos, request.Pagination.PageNumber, request.Pagination.PageSize, total));
        }
    }

    public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, Result<SupplierResponseDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetSupplierByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
        { _uow = uow; _mapper = mapper; }

        public async Task<Result<SupplierResponseDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            Supplier? supplier = await _uow.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
            if (supplier == null) return Result<SupplierResponseDto>.NotFound($"Supplier '{request.SupplierId}' not found.");
            return Result<SupplierResponseDto>.Success(_mapper.Map<SupplierResponseDto>(supplier));
        }
    }
}
