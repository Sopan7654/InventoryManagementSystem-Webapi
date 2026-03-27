// ============================================================
// FILE: src/InventoryManagement.Application/Features/Inventory/Queries/InventoryQueryHandlers.cs
// ============================================================
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Inventory.Queries
{
    /// <summary>Handles GetStockLevelQuery with pagination and optional filters.</summary>
    public class GetStockLevelQueryHandler : IRequestHandler<GetStockLevelQuery, Result<PaginatedResponse<StockLevelResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetStockLevelQueryHandler> _logger;

        public GetStockLevelQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetStockLevelQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedResponse<StockLevelResponseDto>>> Handle(GetStockLevelQuery request, CancellationToken cancellationToken)
        {
            System.Linq.Expressions.Expression<Func<StockLevel, bool>>? filter = null;

            if (!string.IsNullOrEmpty(request.ProductId) && !string.IsNullOrEmpty(request.WarehouseId))
                filter = sl => sl.ProductId == request.ProductId && sl.WarehouseId == request.WarehouseId;
            else if (!string.IsNullOrEmpty(request.ProductId))
                filter = sl => sl.ProductId == request.ProductId;
            else if (!string.IsNullOrEmpty(request.WarehouseId))
                filter = sl => sl.WarehouseId == request.WarehouseId;

            (IEnumerable<StockLevel> items, int totalCount) = await _unitOfWork.StockLevels.GetPagedAsync(
                request.Pagination.PageNumber, request.Pagination.PageSize, filter, cancellationToken: cancellationToken);

            IEnumerable<StockLevelResponseDto> dtos = _mapper.Map<IEnumerable<StockLevelResponseDto>>(items);
            PaginatedResponse<StockLevelResponseDto> response = new(
                dtos, request.Pagination.PageNumber, request.Pagination.PageSize, totalCount);

            return Result<PaginatedResponse<StockLevelResponseDto>>.Success(response);
        }
    }

    /// <summary>Handles GetLowStockReportQuery.</summary>
    public class GetLowStockReportQueryHandler : IRequestHandler<GetLowStockReportQuery, Result<IEnumerable<StockLevelResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetLowStockReportQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<StockLevelResponseDto>>> Handle(GetLowStockReportQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<StockLevel> lowStock = await _unitOfWork.StockLevels.GetLowStockAsync(cancellationToken);
            IEnumerable<StockLevelResponseDto> dtos = _mapper.Map<IEnumerable<StockLevelResponseDto>>(lowStock);
            return Result<IEnumerable<StockLevelResponseDto>>.Success(dtos);
        }
    }

    /// <summary>Handles GetStockTransactionHistoryQuery.</summary>
    public class GetStockTransactionHistoryQueryHandler : IRequestHandler<GetStockTransactionHistoryQuery, Result<PaginatedResponse<StockTransactionResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetStockTransactionHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedResponse<StockTransactionResponseDto>>> Handle(GetStockTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            System.Linq.Expressions.Expression<Func<StockTransaction, bool>>? filter = null;
            if (!string.IsNullOrEmpty(request.ProductId))
                filter = t => t.ProductId == request.ProductId;

            (IEnumerable<StockTransaction> items, int totalCount) = await _unitOfWork.StockTransactions.GetPagedAsync(
                request.Pagination.PageNumber, request.Pagination.PageSize, filter,
                q => q.OrderByDescending(t => t.TransactionDate), cancellationToken);

            IEnumerable<StockTransactionResponseDto> dtos = _mapper.Map<IEnumerable<StockTransactionResponseDto>>(items);
            PaginatedResponse<StockTransactionResponseDto> response = new(
                dtos, request.Pagination.PageNumber, request.Pagination.PageSize, totalCount);

            return Result<PaginatedResponse<StockTransactionResponseDto>>.Success(response);
        }
    }
}
