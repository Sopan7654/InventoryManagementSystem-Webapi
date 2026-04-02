// Features/Reports/Queries/GetAbcAnalysis/GetAbcAnalysisQueryHandler.cs
// ABC Analysis using Pareto Principle (80/20 rule on inventory value).
// Think like Martin Fowler: pure calculation logic in the application layer.
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Inventory.Repository;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Reports.Queries.GetAbcAnalysis
{
    public sealed class GetAbcAnalysisQueryHandler
        : IRequestHandler<GetAbcAnalysisQuery, Result<AbcAnalysisResult>>
    {
        private readonly IStockLevelRepository _stockRepo;
        private readonly IProductRepository    _productRepo;

        public GetAbcAnalysisQueryHandler(
            IStockLevelRepository stockRepo,
            IProductRepository productRepo)
        {
            _stockRepo   = stockRepo;
            _productRepo = productRepo;
        }

        public async Task<Result<AbcAnalysisResult>> Handle(
            GetAbcAnalysisQuery query, CancellationToken ct)
        {
            // Load stock levels and products
            var stockLevels = await _stockRepo.GetAllAsync(ct);
            var products    = await _productRepo.GetAllAsync(ct);

            // Build a cost lookup: ProductId → Cost
            var costMap = products.ToDictionary(p => p.ProductId, p => p.Cost);

            // Aggregate total value per product (sum across all warehouses)
            var productValues = stockLevels
                .GroupBy(sl => sl.ProductId)
                .Select(g =>
                {
                    var totalQty  = g.Sum(sl => sl.QuantityOnHand);
                    var unitCost  = costMap.GetValueOrDefault(g.Key, 0m);
                    return new AbcProduct
                    {
                        ProductId     = g.Key,
                        ProductName   = g.First().ProductName,
                        SKU           = products.FirstOrDefault(p => p.ProductId == g.Key)?.SKU ?? string.Empty,
                        QuantityOnHand = totalQty,
                        UnitCost      = unitCost,
                        TotalValue    = totalQty * unitCost
                    };
                })
                .OrderByDescending(p => p.TotalValue)
                .ToList();

            // Calculate total and cumulative percentages
            decimal totalValue = productValues.Sum(p => p.TotalValue);
            decimal cumulative = 0m;

            var result = new AbcAnalysisResult { TotalInventoryValue = totalValue };

            foreach (var item in productValues)
            {
                cumulative += totalValue == 0 ? 0 : item.TotalValue / totalValue * 100m;
                item.CumulativeValuePercent = Math.Round(cumulative, 2);

                // ABC Classification by cumulative value threshold
                item.AbcClass = cumulative switch
                {
                    <= 80m => "A",
                    <= 95m => "B",
                    _      => "C"
                };

                switch (item.AbcClass)
                {
                    case "A": result.ClassA.Add(item); break;
                    case "B": result.ClassB.Add(item); break;
                    default:  result.ClassC.Add(item); break;
                }
            }

            return Result<AbcAnalysisResult>.Success(result);
        }
    }
}
