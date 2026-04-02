// Features/Products/Commands/ImportProducts/ImportProductsCommandHandler.cs
// Bulk product import — idempotent (skips existing SKUs).
// Deliverable #8: Data import utilities for initial product catalog.
// Think like Jimmy Bogard: one handler, one responsibility, clear result.
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Products.Commands.ImportProducts
{
    public sealed class ImportProductsCommandHandler
        : IRequestHandler<ImportProductsCommand, Result<ImportProductsResult>>
    {
        private readonly IProductRepository _productRepo;

        public ImportProductsCommandHandler(IProductRepository productRepo)
            => _productRepo = productRepo;

        public async Task<Result<ImportProductsResult>> Handle(
            ImportProductsCommand cmd, CancellationToken ct)
        {
            var result = new ImportProductsResult
            {
                TotalSubmitted = cmd.Products.Count
            };

            foreach (var item in cmd.Products)
            {
                // Idempotency check: skip if SKU already exists
                var skuExists = await _productRepo.SKUExistsAsync(item.SKU, null, ct);
                if (skuExists)
                {
                    result.Skipped++;
                    result.SkippedSKUs.Add(item.SKU);
                    continue;
                }

                var productId = Guid.NewGuid().ToString();

                var product = new Product
                {
                    ProductId     = productId,
                    SKU           = item.SKU.Trim().ToUpperInvariant(),
                    ProductName   = item.ProductName.Trim(),
                    Description   = item.Description?.Trim(),
                    CategoryId    = item.CategoryId,
                    UnitOfMeasure = string.IsNullOrWhiteSpace(item.UnitOfMeasure) ? "PCS" : item.UnitOfMeasure.Trim().ToUpperInvariant(),
                    Cost          = item.Cost,
                    ListPrice     = item.ListPrice,
                    IsActive      = true
                };

                await _productRepo.InsertAsync(product, ct);

                result.Imported++;
                result.ImportedProductIds.Add(productId);
            }

            return Result<ImportProductsResult>.Success(result);
        }
    }
}
