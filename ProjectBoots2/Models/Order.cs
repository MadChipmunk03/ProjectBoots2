﻿using System;
using System.Collections.Generic;

namespace ProjectBoots2.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public DateTime? OrderedTime { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Shipping { get; set; } = "Od prodejce";
        public string[] Shippings = new string[] {"Od prodejce", "Česká pošta", "Zásilkovna"};
        public string? Payment { get; set; } = "Hotově";
        public string[] Payments = new string[] {"Hotově", "Kartou", "Bankovní převod" };
        public bool? Finished { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
