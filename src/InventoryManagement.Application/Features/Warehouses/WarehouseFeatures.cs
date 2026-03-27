// ============================================================
// FILE: src/InventoryManagement.Application/Features/Warehouses/WarehouseFeatures.cs
// ============================================================
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Warehouses
{
    public record CreateWarehouseCommand(CreateWarehouseDto Dto) : IRequest<Result<WarehouseResponseDto>>;
    public record UpdateWarehouseCommand(string WarehouseId, UpdateWarehouseDto Dto) : IRequest<Result<WarehouseResponseDto>>;
    public record GetAllWarehousesQuery(PaginationParams Pagination) : IRequest<Result<PaginatedResponse<WarehouseResponseDto>>>, ICacheable
    {
        public string CacheKey => $"{Constants.CacheKeys.AllWarehouses}:{Pagination.PageNumber}:{Pagination.PageSize}";
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(10);
    }
    public record GetWarehouseByIdQuery(string WarehouseId) : IRequest<Result<WarehouseResponseDto>>;

    public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Result<WarehouseResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper; private readonly ICacheService _cache; private readonly ILogger<CreateWarehouseCommandHandler> _logger;
        public CreateWarehouseCommandHandler(IUnitOfWork uow, IMapper mapper, ICacheService cache, ILogger<CreateWarehouseCommandHandler> logger)
        { _uow = uow; _mapper = mapper; _cache = cache; _logger = logger; }

        public async Task<Result<WarehouseResponseDto>> Handle(CreateWarehouseCommand request, CancellationToken ct)
        {
            Warehouse warehouse = _mapper.Map<Warehouse>(request.Dto);
            await _uow.Warehouses.AddAsync(warehouse, ct);
            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveByPatternAsync("warehouses:*", ct);
            _logger.LogInformation("Warehouse created: {WarehouseId}", warehouse.WarehouseId);
            return Result<WarehouseResponseDto>.Success(_mapper.Map<WarehouseResponseDto>(warehouse));
        }
    }

    public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, Result<WarehouseResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper; private readonly ICacheService _cache;
        public UpdateWarehouseCommandHandler(IUnitOfWork uow, IMapper mapper, ICacheService cache) { _uow = uow; _mapper = mapper; _cache = cache; }

        public async Task<Result<WarehouseResponseDto>> Handle(UpdateWarehouseCommand request, CancellationToken ct)
        {
            Warehouse? wh = await _uow.Warehouses.GetByIdAsync(request.WarehouseId, ct);
            if (wh == null) return Result<WarehouseResponseDto>.NotFound($"Warehouse '{request.WarehouseId}' not found.");
            _mapper.Map(request.Dto, wh);
            _uow.Warehouses.Update(wh);
            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveByPatternAsync("warehouses:*", ct);
            return Result<WarehouseResponseDto>.Success(_mapper.Map<WarehouseResponseDto>(wh));
        }
    }

    public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, Result<PaginatedResponse<WarehouseResponseDto>>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetAllWarehousesQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<PaginatedResponse<WarehouseResponseDto>>> Handle(GetAllWarehousesQuery request, CancellationToken ct)
        {
            (IEnumerable<Warehouse> items, int total) = await _uow.Warehouses.GetPagedAsync(
                request.Pagination.PageNumber, request.Pagination.PageSize, cancellationToken: ct);
            IEnumerable<WarehouseResponseDto> dtos = _mapper.Map<IEnumerable<WarehouseResponseDto>>(items);
            return Result<PaginatedResponse<WarehouseResponseDto>>.Success(
                new PaginatedResponse<WarehouseResponseDto>(dtos, request.Pagination.PageNumber, request.Pagination.PageSize, total));
        }
    }

    public class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, Result<WarehouseResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetWarehouseByIdQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<WarehouseResponseDto>> Handle(GetWarehouseByIdQuery request, CancellationToken ct)
        {
            Warehouse? wh = await _uow.Warehouses.GetByIdAsync(request.WarehouseId, ct);
            if (wh == null) return Result<WarehouseResponseDto>.NotFound($"Warehouse '{request.WarehouseId}' not found.");
            return Result<WarehouseResponseDto>.Success(_mapper.Map<WarehouseResponseDto>(wh));
        }
    }
}
