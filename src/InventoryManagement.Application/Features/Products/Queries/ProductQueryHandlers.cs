// ============================================================
// FILE: src/InventoryManagement.Application/Features/Products/Queries/ProductQueryHandlers.cs
// ============================================================
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Products.Queries
{
    /// <summary>Handles GetAllProductsQuery with pagination.</summary>
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PaginatedResponse<ProductSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllProductsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedResponse<ProductSummaryDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<Product> items, int totalCount) = await _unitOfWork.Products.GetPagedAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                cancellationToken: cancellationToken);

            IEnumerable<ProductSummaryDto> dtos = _mapper.Map<IEnumerable<ProductSummaryDto>>(items);

            PaginatedResponse<ProductSummaryDto> response = new(
                dtos, request.Pagination.PageNumber, request.Pagination.PageSize, totalCount);

            _logger.LogDebug("Retrieved {Count}/{Total} products (page {Page})", dtos.Count(), totalCount, request.Pagination.PageNumber);
            return Result<PaginatedResponse<ProductSummaryDto>>.Success(response);
        }
    }

    /// <summary>Handles GetProductByIdQuery.</summary>
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductResponseDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            Product? product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result<ProductResponseDto>.NotFound($"Product '{request.ProductId}' not found.");

            ProductResponseDto response = _mapper.Map<ProductResponseDto>(product);
            return Result<ProductResponseDto>.Success(response);
        }
    }

    /// <summary>Handles GetProductBySKUQuery.</summary>
    public class GetProductBySKUQueryHandler : IRequestHandler<GetProductBySKUQuery, Result<ProductResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductBySKUQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductResponseDto>> Handle(GetProductBySKUQuery request, CancellationToken cancellationToken)
        {
            Product? product = await _unitOfWork.Products.GetBySKUAsync(request.SKU, cancellationToken);
            if (product == null)
                return Result<ProductResponseDto>.NotFound($"Product with SKU '{request.SKU}' not found.");

            ProductResponseDto response = _mapper.Map<ProductResponseDto>(product);
            return Result<ProductResponseDto>.Success(response);
        }
    }
}
