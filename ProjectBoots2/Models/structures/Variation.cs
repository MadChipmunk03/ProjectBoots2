using System;
using System.Collections.Generic;

namespace ProjectBoots2.Models.structures
{
    public partial class Variation
    {
        public Variation()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? Price { get; set; }
        public int? SalePrice { get; set; }
        public int? Size { get; set; }
        public string? Color { get; set; }
        public int? InStock { get; set; }

        public virtual Product? Product { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
