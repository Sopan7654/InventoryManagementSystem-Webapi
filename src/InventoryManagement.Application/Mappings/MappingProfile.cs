// ============================================================
// FILE: src/InventoryManagement.Application/Mappings/MappingProfile.cs
// ============================================================
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Models;

namespace InventoryManagement.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between domain models and DTOs.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Configures all entity-to-DTO and DTO-to-entity mappings.
        /// </summary>
        public MappingProfile()
        {
            // ── Product ──────────────────────────────────────────────
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null));

            CreateMap<Product, ProductSummaryDto>();

            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.SKU, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());

            // ── ProductCategory ──────────────────────────────────────
            CreateMap<ProductCategory, CategoryResponseDto>()
                .ForMember(dest => dest.ParentCategoryName,
                    opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.CategoryName : null))
                .ForMember(dest => dest.ProductCount,
                    opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));

            CreateMap<CreateCategoryDto, ProductCategory>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore());

            CreateMap<UpdateCategoryDto, ProductCategory>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore());

            // ── Supplier ─────────────────────────────────────────────
            CreateMap<Supplier, SupplierResponseDto>()
                .ForMember(dest => dest.PurchaseOrderCount,
                    opt => opt.MapFrom(src => src.PurchaseOrders != null ? src.PurchaseOrders.Count : 0));

            CreateMap<CreateSupplierDto, Supplier>()
                .ForMember(dest => dest.SupplierId, opt => opt.Ignore());

            CreateMap<UpdateSupplierDto, Supplier>()
                .ForMember(dest => dest.SupplierId, opt => opt.Ignore());

            // ── Warehouse ────────────────────────────────────────────
            CreateMap<Warehouse, WarehouseResponseDto>()
                .ForMember(dest => dest.StockLevelCount,
                    opt => opt.MapFrom(src => src.StockLevels != null ? src.StockLevels.Count : 0));

            CreateMap<CreateWarehouseDto, Warehouse>()
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore());

            CreateMap<UpdateWarehouseDto, Warehouse>()
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore());

            // ── StockLevel ───────────────────────────────────────────
            CreateMap<StockLevel, StockLevelResponseDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                .ForMember(dest => dest.ProductSKU,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.SKU : string.Empty))
                .ForMember(dest => dest.WarehouseName,
                    opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.WarehouseName : string.Empty))
                .ForMember(dest => dest.AvailableQuantity,
                    opt => opt.MapFrom(src => src.AvailableQuantity))
                .ForMember(dest => dest.IsLowStock,
                    opt => opt.MapFrom(src => src.IsLowStock));

            // ── StockTransaction ─────────────────────────────────────
            CreateMap<StockTransaction, StockTransactionResponseDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                .ForMember(dest => dest.WarehouseName,
                    opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.WarehouseName : string.Empty));

            // ── Batch ────────────────────────────────────────────────
            CreateMap<Batch, BatchResponseDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                .ForMember(dest => dest.WarehouseName,
                    opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.WarehouseName : string.Empty))
                .ForMember(dest => dest.ExpiryStatus,
                    opt => opt.MapFrom(src => src.ExpiryStatus));

            CreateMap<CreateBatchDto, Batch>()
                .ForMember(dest => dest.BatchId, opt => opt.Ignore());

            CreateMap<UpdateBatchDto, Batch>()
                .ForMember(dest => dest.BatchId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore())
                .ForMember(dest => dest.BatchNumber, opt => opt.Ignore());

            // ── PurchaseOrder ────────────────────────────────────────
            CreateMap<PurchaseOrder, PurchaseOrderResponseDto>()
                .ForMember(dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : string.Empty))
                .ForMember(dest => dest.ItemCount,
                    opt => opt.MapFrom(src => src.Items != null ? src.Items.Count : 0))
                .ForMember(dest => dest.TotalAmount,
                    opt => opt.MapFrom(src => src.Items != null
                        ? src.Items.Sum(i => i.QuantityOrdered * i.UnitPrice) : 0));

            CreateMap<PurchaseOrderItem, PurchaseOrderItemResponseDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                .ForMember(dest => dest.LineTotal,
                    opt => opt.MapFrom(src => src.LineTotal));

            CreateMap<AddPurchaseOrderItemDto, PurchaseOrderItem>()
                .ForMember(dest => dest.POItemId, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.Ignore());
        }
    }
}
