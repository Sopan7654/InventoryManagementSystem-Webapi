// ============================================================
// FILE: src/InventoryManagement.Application/Features/Products/Commands/ProductCommandHandlers.cs
// ============================================================
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Events;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Products.Commands
{
    /// <summary>Handles CreateProductCommand.</summary>
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductCommandHandler> _logger;
        private readonly ICacheService _cacheService;
        private readonly IEventPublisher _eventPublisher;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<CreateProductCommandHandler> logger, ICacheService cacheService, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<ProductResponseDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.Products.SKUExistsAsync(request.Dto.SKU, cancellationToken))
            {
                _logger.LogWarning("Attempted to create product with duplicate SKU: {SKU}", request.Dto.SKU);
                return Result<ProductResponseDto>.ConflictError($"A product with SKU '{request.Dto.SKU}' already exists.");
            }

            Product product = _mapper.Map<Product>(request.Dto);
            await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(Constants.CacheKeys.AllProducts, cancellationToken);

            await _eventPublisher.PublishAsync(
                new ProductCreatedEvent(product.ProductId, product.SKU, product.ProductName), cancellationToken);

            _logger.LogInformation("Product created: {ProductId} ({SKU})", product.ProductId, product.SKU);

            ProductResponseDto response = _mapper.Map<ProductResponseDto>(product);
            return Result<ProductResponseDto>.Success(response);
        }
    }

    /// <summary>Handles UpdateProductCommand.</summary>
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        private readonly ICacheService _cacheService;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<UpdateProductCommandHandler> logger, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<Result<ProductResponseDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            Product? product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result<ProductResponseDto>.NotFound($"Product '{request.ProductId}' not found.");

            _mapper.Map(request.Dto, product);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(Constants.CacheKeys.AllProducts, cancellationToken);
            await _cacheService.RemoveAsync(string.Format(Constants.CacheKeys.ProductById, request.ProductId), cancellationToken);

            _logger.LogInformation("Product updated: {ProductId}", request.ProductId);

            ProductResponseDto response = _mapper.Map<ProductResponseDto>(product);
            return Result<ProductResponseDto>.Success(response);
        }
    }

    /// <summary>Handles DeleteProductCommand (soft delete).</summary>
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteProductCommandHandler> _logger;
        private readonly ICacheService _cacheService;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork,
            ILogger<DeleteProductCommandHandler> logger, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            Product? product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result<bool>.NotFound($"Product '{request.ProductId}' not found.");

            product.IsActive = false;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(Constants.CacheKeys.AllProducts, cancellationToken);
            await _cacheService.RemoveAsync(string.Format(Constants.CacheKeys.ProductById, request.ProductId), cancellationToken);

            _logger.LogInformation("Product soft-deleted: {ProductId}", request.ProductId);
            return Result<bool>.Success(true);
        }
    }

    /// <summary>Handles ToggleProductStatusCommand.</summary>
    public class ToggleProductStatusCommandHandler : IRequestHandler<ToggleProductStatusCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ToggleProductStatusCommandHandler> _logger;
        private readonly ICacheService _cacheService;

        public ToggleProductStatusCommandHandler(IUnitOfWork unitOfWork,
            ILogger<ToggleProductStatusCommandHandler> logger, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<Result<bool>> Handle(ToggleProductStatusCommand request, CancellationToken cancellationToken)
        {
            Product? product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Result<bool>.NotFound($"Product '{request.ProductId}' not found.");

            product.IsActive = request.IsActive;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(Constants.CacheKeys.AllProducts, cancellationToken);
            await _cacheService.RemoveAsync(string.Format(Constants.CacheKeys.ProductById, request.ProductId), cancellationToken);

            _logger.LogInformation("Product {ProductId} status changed to {Status}", request.ProductId, request.IsActive);
            return Result<bool>.Success(true);
        }
    }
}
