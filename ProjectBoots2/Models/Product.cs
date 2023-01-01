using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectBoots2.Models
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

        [Required]
        [RegularExpression("^[A-Z].*")] //starts with capital letter
        public string? Title { get; set; }

        [Required]
        [MinLength(512)]
        public string? Description { get; set; }

        [Required]
        public string? ParamType { get; set; }

        [Required]
        public string? ParamMaterial { get; set; }

        [Required]
        public string? ParamPurpose { get; set; }

        [Required]
        public string? ParamCode { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<Variation> Variations { get; set; }
    }
}
