using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CategoryProduct> CategoryProducts { get; set; } = new List<CategoryProduct>();
    }
}