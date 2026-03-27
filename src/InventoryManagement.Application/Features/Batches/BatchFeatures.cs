// ============================================================
// FILE: src/InventoryManagement.Application/Features/Batches/BatchFeatures.cs
// ============================================================
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Batches
{
    public record CreateBatchCommand(CreateBatchDto Dto) : IRequest<Result<BatchResponseDto>>;
    public record UpdateBatchCommand(string BatchId, UpdateBatchDto Dto) : IRequest<Result<BatchResponseDto>>;
    public record GetBatchesByProductQuery(string ProductId) : IRequest<Result<IEnumerable<BatchResponseDto>>>;
    public record GetExpiringBatchesQuery(int DaysAhead = 30) : IRequest<Result<IEnumerable<BatchResponseDto>>>, ICacheable
    {
        public string CacheKey => string.Format(Constants.CacheKeys.ExpiringBatches, DaysAhead);
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(10);
    }

    public class CreateBatchCommandHandler : IRequestHandler<CreateBatchCommand, Result<BatchResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper; private readonly ILogger<CreateBatchCommandHandler> _logger;
        public CreateBatchCommandHandler(IUnitOfWork uow, IMapper mapper, ILogger<CreateBatchCommandHandler> logger)
        { _uow = uow; _mapper = mapper; _logger = logger; }

        public async Task<Result<BatchResponseDto>> Handle(CreateBatchCommand request, CancellationToken ct)
        {
            Product? product = await _uow.Products.GetByIdAsync(request.Dto.ProductId, ct);
            if (product == null) return Result<BatchResponseDto>.NotFound($"Product '{request.Dto.ProductId}' not found.");

            Warehouse? wh = await _uow.Warehouses.GetByIdAsync(request.Dto.WarehouseId, ct);
            if (wh == null) return Result<BatchResponseDto>.NotFound($"Warehouse '{request.Dto.WarehouseId}' not found.");

            Batch batch = _mapper.Map<Batch>(request.Dto);
            await _uow.Batches.AddAsync(batch, ct);
            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("Batch created: {BatchId} ({BatchNumber})", batch.BatchId, batch.BatchNumber);
            return Result<BatchResponseDto>.Success(_mapper.Map<BatchResponseDto>(batch));
        }
    }

    public class UpdateBatchCommandHandler : IRequestHandler<UpdateBatchCommand, Result<BatchResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public UpdateBatchCommandHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<BatchResponseDto>> Handle(UpdateBatchCommand request, CancellationToken ct)
        {
            Batch? batch = await _uow.Batches.GetByIdAsync(request.BatchId, ct);
            if (batch == null) return Result<BatchResponseDto>.NotFound($"Batch '{request.BatchId}' not found.");
            _mapper.Map(request.Dto, batch);
            _uow.Batches.Update(batch);
            await _uow.SaveChangesAsync(ct);
            return Result<BatchResponseDto>.Success(_mapper.Map<BatchResponseDto>(batch));
        }
    }

    public class GetBatchesByProductQueryHandler : IRequestHandler<GetBatchesByProductQuery, Result<IEnumerable<BatchResponseDto>>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetBatchesByProductQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<IEnumerable<BatchResponseDto>>> Handle(GetBatchesByProductQuery request, CancellationToken ct)
        {
            IEnumerable<Batch> batches = await _uow.Batches.GetByProductAsync(request.ProductId, ct);
            return Result<IEnumerable<BatchResponseDto>>.Success(_mapper.Map<IEnumerable<BatchResponseDto>>(batches));
        }
    }

    public class GetExpiringBatchesQueryHandler : IRequestHandler<GetExpiringBatchesQuery, Result<IEnumerable<BatchResponseDto>>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetExpiringBatchesQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<IEnumerable<BatchResponseDto>>> Handle(GetExpiringBatchesQuery request, CancellationToken ct)
        {
            IEnumerable<Batch> batches = await _uow.Batches.GetExpiringSoonAsync(request.DaysAhead, ct);
            return Result<IEnumerable<BatchResponseDto>>.Success(_mapper.Map<IEnumerable<BatchResponseDto>>(batches));
        }
    }
}
