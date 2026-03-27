// ============================================================
// FILE: src/InventoryManagement.Application/Features/PurchaseOrders/PurchaseOrderFeatures.cs
// ============================================================
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Events;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.PurchaseOrders
{
    public record CreatePurchaseOrderCommand(CreatePurchaseOrderDto Dto) : IRequest<Result<PurchaseOrderResponseDto>>;
    public record UpdatePurchaseOrderCommand(string PurchaseOrderId, UpdatePurchaseOrderDto Dto) : IRequest<Result<PurchaseOrderResponseDto>>;
    public record ApprovePurchaseOrderCommand(string PurchaseOrderId) : IRequest<Result<bool>>;
    public record ReceivePurchaseOrderCommand(string PurchaseOrderId, string WarehouseId) : IRequest<Result<string>>;
    public record GetAllPurchaseOrdersQuery(PaginationParams Pagination) : IRequest<Result<PaginatedResponse<PurchaseOrderResponseDto>>>;
    public record GetPurchaseOrderByIdQuery(string PurchaseOrderId) : IRequest<Result<PurchaseOrderResponseDto>>;

    public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Result<PurchaseOrderResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper; private readonly ILogger<CreatePurchaseOrderCommandHandler> _logger;
        public CreatePurchaseOrderCommandHandler(IUnitOfWork uow, IMapper mapper, ILogger<CreatePurchaseOrderCommandHandler> logger)
        { _uow = uow; _mapper = mapper; _logger = logger; }

        public async Task<Result<PurchaseOrderResponseDto>> Handle(CreatePurchaseOrderCommand request, CancellationToken ct)
        {
            Supplier? supplier = await _uow.Suppliers.GetByIdAsync(request.Dto.SupplierId, ct);
            if (supplier == null) return Result<PurchaseOrderResponseDto>.NotFound($"Supplier '{request.Dto.SupplierId}' not found.");

            PurchaseOrder po = new()
            {
                SupplierId = request.Dto.SupplierId,
                Status = PurchaseOrderStatus.PENDING
            };

            foreach (AddPurchaseOrderItemDto itemDto in request.Dto.Items)
            {
                Product? product = await _uow.Products.GetByIdAsync(itemDto.ProductId, ct);
                if (product == null) return Result<PurchaseOrderResponseDto>.NotFound($"Product '{itemDto.ProductId}' not found.");

                po.Items.Add(_mapper.Map<PurchaseOrderItem>(itemDto));
            }

            await _uow.PurchaseOrders.AddAsync(po, ct);
            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("PO created: {POId} for supplier {SupplierId}", po.PurchaseOrderId, po.SupplierId);

            PurchaseOrder? created = await _uow.PurchaseOrders.GetWithItemsAsync(po.PurchaseOrderId, ct);
            return Result<PurchaseOrderResponseDto>.Success(_mapper.Map<PurchaseOrderResponseDto>(created));
        }
    }

    public class ApprovePurchaseOrderCommandHandler : IRequestHandler<ApprovePurchaseOrderCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow; private readonly IEventPublisher _events; private readonly ILogger<ApprovePurchaseOrderCommandHandler> _logger;
        public ApprovePurchaseOrderCommandHandler(IUnitOfWork uow, IEventPublisher events, ILogger<ApprovePurchaseOrderCommandHandler> logger)
        { _uow = uow; _events = events; _logger = logger; }

        public async Task<Result<bool>> Handle(ApprovePurchaseOrderCommand request, CancellationToken ct)
        {
            PurchaseOrder? po = await _uow.PurchaseOrders.GetByIdAsync(request.PurchaseOrderId, ct);
            if (po == null) return Result<bool>.NotFound($"Purchase order '{request.PurchaseOrderId}' not found.");
            if (po.Status != PurchaseOrderStatus.PENDING)
                return Result<bool>.ValidationError($"Cannot approve PO in '{po.Status}' status. Must be PENDING.");

            po.Status = PurchaseOrderStatus.APPROVED;
            _uow.PurchaseOrders.Update(po);
            await _uow.SaveChangesAsync(ct);

            await _events.PublishAsync(new PurchaseOrderApprovedEvent(po.PurchaseOrderId, po.SupplierId), ct);
            _logger.LogInformation("PO approved: {POId}", po.PurchaseOrderId);
            return Result<bool>.Success(true);
        }
    }

    public class ReceivePurchaseOrderCommandHandler : IRequestHandler<ReceivePurchaseOrderCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow; private readonly ICacheService _cache; private readonly IEventPublisher _events;
        private readonly ILogger<ReceivePurchaseOrderCommandHandler> _logger;
        public ReceivePurchaseOrderCommandHandler(IUnitOfWork uow, ICacheService cache, IEventPublisher events, ILogger<ReceivePurchaseOrderCommandHandler> logger)
        { _uow = uow; _cache = cache; _events = events; _logger = logger; }

        public async Task<Result<string>> Handle(ReceivePurchaseOrderCommand request, CancellationToken ct)
        {
            PurchaseOrder? po = await _uow.PurchaseOrders.GetWithItemsAsync(request.PurchaseOrderId, ct);
            if (po == null) return Result<string>.NotFound($"Purchase order '{request.PurchaseOrderId}' not found.");
            if (po.Status != PurchaseOrderStatus.APPROVED)
                return Result<string>.ValidationError($"Cannot receive PO in '{po.Status}' status. Must be APPROVED.");

            Warehouse? wh = await _uow.Warehouses.GetByIdAsync(request.WarehouseId, ct);
            if (wh == null) return Result<string>.NotFound($"Warehouse '{request.WarehouseId}' not found.");

            foreach (PurchaseOrderItem item in po.Items)
            {
                StockLevel? stock = await _uow.StockLevels.GetByProductAndWarehouseAsync(item.ProductId, request.WarehouseId, ct);
                decimal previousQty = stock?.QuantityOnHand ?? 0;

                if (stock != null)
                {
                    stock.QuantityOnHand += item.QuantityOrdered;
                    _uow.StockLevels.Update(stock);
                }
                else
                {
                    stock = new StockLevel { ProductId = item.ProductId, WarehouseId = request.WarehouseId, QuantityOnHand = item.QuantityOrdered };
                    await _uow.StockLevels.AddAsync(stock, ct);
                }

                await _uow.StockTransactions.AddAsync(new StockTransaction
                {
                    ProductId = item.ProductId, WarehouseId = request.WarehouseId,
                    TransactionType = TransactionType.PURCHASE, Quantity = item.QuantityOrdered,
                    Reference = $"PO-{po.PurchaseOrderId}"
                }, ct);

                await _cache.RemoveAsync(string.Format(Constants.CacheKeys.StockLevel, item.ProductId, request.WarehouseId), ct);

                await _events.PublishAsync(new StockLevelChangedEvent(
                    item.ProductId, request.WarehouseId, previousQty, stock.QuantityOnHand, stock.ReorderLevel), ct);
            }

            po.Status = PurchaseOrderStatus.RECEIVED;
            _uow.PurchaseOrders.Update(po);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("PO received: {POId} into warehouse {WarehouseId}", po.PurchaseOrderId, request.WarehouseId);
            return Result<string>.Success($"PO {po.PurchaseOrderId} received successfully. {po.Items.Count} items stocked in.");
        }
    }

    public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, Result<PurchaseOrderResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public UpdatePurchaseOrderCommandHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<PurchaseOrderResponseDto>> Handle(UpdatePurchaseOrderCommand request, CancellationToken ct)
        {
            PurchaseOrder? po = await _uow.PurchaseOrders.GetByIdAsync(request.PurchaseOrderId, ct);
            if (po == null) return Result<PurchaseOrderResponseDto>.NotFound($"PO '{request.PurchaseOrderId}' not found.");
            if (po.Status != PurchaseOrderStatus.PENDING)
                return Result<PurchaseOrderResponseDto>.ValidationError("Can only update PENDING purchase orders.");

            po.SupplierId = request.Dto.SupplierId;
            _uow.PurchaseOrders.Update(po);
            await _uow.SaveChangesAsync(ct);
            return Result<PurchaseOrderResponseDto>.Success(_mapper.Map<PurchaseOrderResponseDto>(po));
        }
    }

    public class GetAllPurchaseOrdersQueryHandler : IRequestHandler<GetAllPurchaseOrdersQuery, Result<PaginatedResponse<PurchaseOrderResponseDto>>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetAllPurchaseOrdersQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<PaginatedResponse<PurchaseOrderResponseDto>>> Handle(GetAllPurchaseOrdersQuery request, CancellationToken ct)
        {
            (IEnumerable<PurchaseOrder> items, int total) = await _uow.PurchaseOrders.GetPagedAsync(
                request.Pagination.PageNumber, request.Pagination.PageSize, cancellationToken: ct);
            IEnumerable<PurchaseOrderResponseDto> dtos = _mapper.Map<IEnumerable<PurchaseOrderResponseDto>>(items);
            return Result<PaginatedResponse<PurchaseOrderResponseDto>>.Success(
                new PaginatedResponse<PurchaseOrderResponseDto>(dtos, request.Pagination.PageNumber, request.Pagination.PageSize, total));
        }
    }

    public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, Result<PurchaseOrderResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetPurchaseOrderByIdQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<PurchaseOrderResponseDto>> Handle(GetPurchaseOrderByIdQuery request, CancellationToken ct)
        {
            PurchaseOrder? po = await _uow.PurchaseOrders.GetWithItemsAsync(request.PurchaseOrderId, ct);
            if (po == null) return Result<PurchaseOrderResponseDto>.NotFound($"PO '{request.PurchaseOrderId}' not found.");
            return Result<PurchaseOrderResponseDto>.Success(_mapper.Map<PurchaseOrderResponseDto>(po));
        }
    }
}
