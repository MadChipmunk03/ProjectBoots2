using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectBoots2.Models
{
    public partial class Administrator
    {
        public int Id { get; set; }

        [Required]
        public string? Username { get; set; }

        public string? Password { get; set; }
    }
}
