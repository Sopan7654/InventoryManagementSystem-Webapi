// Features/Products/Commands/ImportProducts/ImportProductsCommand.cs
using InventoryManagementSystem.Common.Models;
using MediatR;

namespace InventoryManagementSystem.Features.Products.Commands.ImportProducts
{
    /// <summary>
    /// Bulk import command for initial product catalog loading.
    /// Deliverable #8: Data import utilities.
    /// Skips products with duplicate SKUs (idempotent import).
    /// </summary>
    public record ImportProductsCommand(
        List<ProductImportItem> Products
    ) : IRequest<Result<ImportProductsResult>>;

    public class ProductImportItem
    {
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CategoryId { get; set; }
        public string UnitOfMeasure { get; set; } = "PCS";
        public decimal Cost { get; set; }
        public decimal ListPrice { get; set; }
    }

    public class ImportProductsResult
    {
        public int TotalSubmitted { get; set; }
        public int Imported { get; set; }
        public int Skipped { get; set; }
        public List<string> SkippedSKUs { get; set; } = new();
        public List<string> ImportedProductIds { get; set; } = new();
    }
}
