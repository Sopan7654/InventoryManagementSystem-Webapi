// ============================================================
// FILE: src/InventoryManagement.Shared/PaginatedResponse.cs
// ============================================================
namespace InventoryManagement.Shared
{
    /// <summary>
    /// A paginated response wrapper for list endpoints.
    /// </summary>
    /// <typeparam name="T">The type of items in the page.</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>The items on the current page.</summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>Current page number (1-based).</summary>
        public int PageNumber { get; set; }

        /// <summary>Number of items per page.</summary>
        public int PageSize { get; set; }

        /// <summary>Total number of items across all pages.</summary>
        public int TotalCount { get; set; }

        /// <summary>Total number of pages.</summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>Whether there is a next page.</summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>Whether there is a previous page.</summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Creates a new paginated response.
        /// </summary>
        public PaginatedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }

    /// <summary>
    /// Parameters for pagination queries.
    /// </summary>
    public class PaginationParams
    {
        private int _pageNumber = 1;
        private int _pageSize = 20;
        private const int MaxPageSize = 100;

        /// <summary>Page number (minimum 1).</summary>
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>Items per page (maximum 100).</summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
        }
    }
}
