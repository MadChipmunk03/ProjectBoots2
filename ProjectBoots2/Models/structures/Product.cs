using System;
using System.Collections.Generic;

namespace ProjectBoots2.Models.structures
{
    public partial class Product
    {
        public Product()
        {
            ProductImages = new HashSet<ProductImage>();
            Variations = new HashSet<Variation>();
        }

        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ParamType { get; set; }
        public string? ParamMaterial { get; set; }
        public string? ParamPurpose { get; set; }
        public string? ParamCode { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<Variation> Variations { get; set; }
    }
}
