using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectBoots2.Models
{
    public class HomeModel
    {
        public bool ShowAll { get; set; } = false;
        public int? CategoryId { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
        public ProductSortTypes SortType { get; set; } = ProductSortTypes.Default;
    }

    public enum ProductSortTypes
    {
        Default,
        ExpensiveToCheap,
        CheapToExpensive
    }
}
