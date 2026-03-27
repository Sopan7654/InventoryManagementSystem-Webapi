// ============================================================
// FILE: src/InventoryManagement.Application/Features/Categories/CategoryFeatures.cs
// ============================================================
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Categories
{
    public record CreateCategoryCommand(CreateCategoryDto Dto) : IRequest<Result<CategoryResponseDto>>;
    public record UpdateCategoryCommand(string CategoryId, UpdateCategoryDto Dto) : IRequest<Result<CategoryResponseDto>>;
    public record DeleteCategoryCommand(string CategoryId) : IRequest<Result<bool>>;
    public record GetAllCategoriesQuery() : IRequest<Result<IEnumerable<CategoryResponseDto>>>, ICacheable
    {
        public string CacheKey => Constants.CacheKeys.AllCategories;
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(30);
    }
    public record GetCategoryByIdQuery(string CategoryId) : IRequest<Result<CategoryResponseDto>>;

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper; private readonly ICacheService _cache; private readonly ILogger<CreateCategoryCommandHandler> _logger;
        public CreateCategoryCommandHandler(IUnitOfWork uow, IMapper mapper, ICacheService cache, ILogger<CreateCategoryCommandHandler> logger)
        { _uow = uow; _mapper = mapper; _cache = cache; _logger = logger; }

        public async Task<Result<CategoryResponseDto>> Handle(CreateCategoryCommand request, CancellationToken ct)
        {
            ProductCategory category = _mapper.Map<ProductCategory>(request.Dto);
            await _uow.Categories.AddAsync(category, ct);
            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveByPatternAsync("categories:*", ct);
            _logger.LogInformation("Category created: {CategoryId}", category.CategoryId);
            return Result<CategoryResponseDto>.Success(_mapper.Map<CategoryResponseDto>(category));
        }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper; private readonly ICacheService _cache;
        public UpdateCategoryCommandHandler(IUnitOfWork uow, IMapper mapper, ICacheService cache) { _uow = uow; _mapper = mapper; _cache = cache; }

        public async Task<Result<CategoryResponseDto>> Handle(UpdateCategoryCommand request, CancellationToken ct)
        {
            ProductCategory? cat = await _uow.Categories.GetByIdAsync(request.CategoryId, ct);
            if (cat == null) return Result<CategoryResponseDto>.NotFound($"Category '{request.CategoryId}' not found.");
            _mapper.Map(request.Dto, cat);
            _uow.Categories.Update(cat);
            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveByPatternAsync("categories:*", ct);
            return Result<CategoryResponseDto>.Success(_mapper.Map<CategoryResponseDto>(cat));
        }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow; private readonly ICacheService _cache;
        public DeleteCategoryCommandHandler(IUnitOfWork uow, ICacheService cache) { _uow = uow; _cache = cache; }

        public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            ProductCategory? cat = await _uow.Categories.GetByIdAsync(request.CategoryId, ct);
            if (cat == null) return Result<bool>.NotFound($"Category '{request.CategoryId}' not found.");
            _uow.Categories.Delete(cat);
            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveByPatternAsync("categories:*", ct);
            return Result<bool>.Success(true);
        }
    }

    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<IEnumerable<CategoryResponseDto>>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetAllCategoriesQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<IEnumerable<CategoryResponseDto>>> Handle(GetAllCategoriesQuery request, CancellationToken ct)
        {
            IEnumerable<ProductCategory> categories = await _uow.Categories.GetAllAsync(ct);
            return Result<IEnumerable<CategoryResponseDto>>.Success(_mapper.Map<IEnumerable<CategoryResponseDto>>(categories));
        }
    }

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponseDto>>
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public GetCategoryByIdQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<Result<CategoryResponseDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
        {
            ProductCategory? cat = await _uow.Categories.GetByIdAsync(request.CategoryId, ct);
            if (cat == null) return Result<CategoryResponseDto>.NotFound($"Category '{request.CategoryId}' not found.");
            return Result<CategoryResponseDto>.Success(_mapper.Map<CategoryResponseDto>(cat));
        }
    }
}
