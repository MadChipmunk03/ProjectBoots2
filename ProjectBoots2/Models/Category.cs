using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectBoots2.Models
{
    public partial class Category
    {
        public Category()
        {
            Children = new HashSet<Category>();
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public int? ParentId { get; set; }

        [Required]
        [RegularExpression("^[A-Z].*")] //starts with capital letter
        public string? CategoryName { get; set; }
        public string CategoryPath = "";

        public virtual Category? Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
